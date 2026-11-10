using System.Globalization;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

namespace GUICOLONYPROJECT;

public partial class Form1 : Form
{
    private const string GradesPath = "grades.txt";
    private const string ColonyPath = "colony.txt";
    private const string UniversitiesPath = "universities.txt";

    // Your own university: the home base of your colony (not a target, so it is not in University.All)
    private const string HomeName = "Université Ibn Khaldoun (UIK)";
    private const string HomeTag = "home";

    // Approximate: Montplaisir district, rue Elless. For the exact spot, right-click the building
    // on the map and paste the coordinates here.
    private const double HomeLatitude = 36.8170;
    private const double HomeLongitude = 10.1850;

    private readonly GradeTracker tracker = new GradeTracker();
    private readonly Colony colony;

    private readonly TabControl tabs = new TabControl();

    // Colony tab
    private readonly Label turnLabel = new Label();
    private readonly Label coinsLabel = new Label();
    private readonly Label incomeLabel = new Label();
    private readonly Label averageLabel = new Label();
    private readonly Label resourcesLabel = new Label();
    private readonly Label logLabel = new Label();
    private readonly Button endTurnButton = new Button();

    // Grades tab
    private readonly Label gradesSummaryLabel = new Label();
    private readonly ListView gradesList = new ListView();
    private readonly TextBox nameBox = new TextBox();
    private readonly NumericUpDown gradeBox = new NumericUpDown();
    private readonly NumericUpDown coefBox = new NumericUpDown();

    // Buildings tab
    private readonly Label buildingsStatusLabel = new Label();
    private readonly ListView buildingsList = new ListView();

    // Map tab
    private readonly GMapControl map = new GMapControl();
    private readonly GMapOverlay universityOverlay = new GMapOverlay("universities");
    private readonly Label mapStatusLabel = new Label();

    public Form1()
    {
        InitializeComponent();

        // Load the game with the same classes the console version uses
        tracker.Load(GradesPath);
        colony = Colony.Load(ColonyPath);
        University.LoadAll(UniversitiesPath);

        Text = "Grade Colony";
        ClientSize = new Size(900, 640);
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 11F);

        tabs.Dock = DockStyle.Fill;
        tabs.TabPages.Add(BuildColonyTab());
        tabs.TabPages.Add(BuildGradesTab());
        tabs.TabPages.Add(BuildBuildingsTab());
        tabs.TabPages.Add(BuildMapTab());
        Controls.Add(tabs);

        RefreshGrades();
        RefreshBuildings();

        // Save automatically when the window closes
        FormClosing += (sender, e) => SaveAll();
    }

    private void SaveAll()
    {
        tracker.Save(GradesPath);
        colony.Save(ColonyPath);
        University.SaveAll(UniversitiesPath);
    }

    // Turns a set of resources into text, like "5 Food, 3 Wood"
    private static string FormatResources(Dictionary<ResourceType, int> amounts)
    {
        var parts = new List<string>();

        foreach (var pair in amounts)
        {
            parts.Add($"{pair.Value} {pair.Key}");
        }

        return string.Join(", ", parts);
    }

    // ---------- Colony tab ----------

    private TabPage BuildColonyTab()
    {
        var page = new TabPage("Colony");

        var panel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Padding = new Padding(20)
        };

        foreach (Label label in new[] { turnLabel, coinsLabel, incomeLabel, averageLabel, resourcesLabel, logLabel })
        {
            label.AutoSize = true;
            label.MaximumSize = new Size(800, 0);
            label.Margin = new Padding(0, 0, 0, 12);
            panel.Controls.Add(label);
        }

        turnLabel.Font = new Font(Font, FontStyle.Bold);
        coinsLabel.Font = new Font(Font, FontStyle.Bold);
        logLabel.ForeColor = Color.DimGray;

        endTurnButton.Text = "End turn (collect coins)";
        endTurnButton.AutoSize = true;
        endTurnButton.Padding = new Padding(10, 5, 10, 5);
        endTurnButton.Click += EndTurnButton_Click;
        panel.Controls.Add(endTurnButton);

        page.Controls.Add(panel);
        return page;
    }

    private void RefreshStats()
    {
        double average = tracker.CalculateAverage();
        int subjectCount = tracker.Subjects.Count;
        int income = colony.CalculateIncome(average, subjectCount);

        turnLabel.Text = $"Turn {colony.Turn}";
        coinsLabel.Text = $"Coins: {colony.Coins}";
        incomeLabel.Text = $"Income per turn: {income}";
        averageLabel.Text = $"Average: {average:0.00} ({subjectCount} subjects)";

        var lines = new List<string>();
        foreach (var pair in colony.Resources)
        {
            lines.Add($"{pair.Key}: {pair.Value}");
        }
        resourcesLabel.Text = "Resources" + Environment.NewLine + string.Join(Environment.NewLine, lines);
    }

    private void EndTurnButton_Click(object? sender, EventArgs e)
    {
        int income = colony.EndTurn(tracker.CalculateAverage(), tracker.Subjects.Count);

        var message = new List<string> { $"Turn {colony.Turn - 1} finished. You earned {income} coins." };
        message.AddRange(colony.Log);
        logLabel.Text = string.Join(Environment.NewLine, message);

        RefreshStats();
        RefreshBuildings();
        SaveAll();
    }

    // ---------- Grades tab ----------

    private TabPage BuildGradesTab()
    {
        var page = new TabPage("Grades");

        // The list (fills the middle)
        gradesList.Dock = DockStyle.Fill;
        gradesList.View = View.Details;
        gradesList.FullRowSelect = true;
        gradesList.GridLines = true;
        gradesList.MultiSelect = false;
        gradesList.HideSelection = false;
        gradesList.Columns.Add("Subject", 230);
        gradesList.Columns.Add("Grade", 80);
        gradesList.Columns.Add("Coef", 80);
        gradesList.Columns.Add("Result", 130);
        gradesList.SelectedIndexChanged += GradesList_SelectedIndexChanged;

        // Summary at the top
        gradesSummaryLabel.Dock = DockStyle.Top;
        gradesSummaryLabel.Height = 40;
        gradesSummaryLabel.Padding = new Padding(10, 10, 0, 0);
        gradesSummaryLabel.Font = new Font(Font, FontStyle.Bold);

        // Input row at the bottom
        var inputs = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            AutoSize = true,
            WrapContents = true,
            Padding = new Padding(10)
        };

        nameBox.Width = 170;
        nameBox.PlaceholderText = "Subject name";

        gradeBox.Minimum = 0;
        gradeBox.Maximum = 20;
        gradeBox.DecimalPlaces = 2;
        gradeBox.Increment = 0.5M;
        gradeBox.Width = 80;

        coefBox.Minimum = 0.1M;
        coefBox.Maximum = 10;
        coefBox.DecimalPlaces = 1;
        coefBox.Increment = 0.5M;
        coefBox.Value = 1;
        coefBox.Width = 70;

        var addButton = new Button { Text = "Add subject", AutoSize = true };
        var updateButton = new Button { Text = "Update grade", AutoSize = true };
        var removeButton = new Button { Text = "Remove", AutoSize = true };
        addButton.Click += AddButton_Click;
        updateButton.Click += UpdateButton_Click;
        removeButton.Click += RemoveButton_Click;

        inputs.Controls.Add(new Label { Text = "Name", AutoSize = true, Margin = new Padding(0, 6, 0, 0) });
        inputs.Controls.Add(nameBox);
        inputs.Controls.Add(new Label { Text = "Grade /20", AutoSize = true, Margin = new Padding(10, 6, 0, 0) });
        inputs.Controls.Add(gradeBox);
        inputs.Controls.Add(new Label { Text = "Coef", AutoSize = true, Margin = new Padding(10, 6, 0, 0) });
        inputs.Controls.Add(coefBox);
        inputs.SetFlowBreak(coefBox, true);
        inputs.Controls.Add(addButton);
        inputs.Controls.Add(updateButton);
        inputs.Controls.Add(removeButton);

        // The middle control is added first, then the ones docked to the edges
        page.Controls.Add(gradesList);
        page.Controls.Add(inputs);
        page.Controls.Add(gradesSummaryLabel);
        return page;
    }

    // Rebuilds the list from the tracker and updates the numbers on both tabs
    private void RefreshGrades()
    {
        gradesList.Items.Clear();

        foreach (Subject s in tracker.Subjects)
        {
            gradesList.Items.Add(new ListViewItem(new[]
            {
                s.Name,
                s.Grade.ToString("0.##"),
                s.Coefficient.ToString("0.##"),
                s.Mention
            }));
        }

        double average = tracker.CalculateAverage();
        gradesSummaryLabel.Text = tracker.Subjects.Count == 0
            ? "No subjects yet. Add one below."
            : $"Average: {average:0.00} - {Subject.GetMention(average)}";

        RefreshStats();
    }

    private int SelectedGradeIndex()
    {
        return gradesList.SelectedIndices.Count > 0 ? gradesList.SelectedIndices[0] : -1;
    }

    // Selecting a subject fills the boxes with its values
    private void GradesList_SelectedIndexChanged(object? sender, EventArgs e)
    {
        int index = SelectedGradeIndex();
        if (index < 0) return;

        Subject s = tracker.Subjects[index];
        nameBox.Text = s.Name;
        gradeBox.Value = (decimal)Math.Clamp(s.Grade, 0, 20);
        coefBox.Value = (decimal)Math.Clamp(s.Coefficient, 0.1, 10);
    }

    private void AddButton_Click(object? sender, EventArgs e)
    {
        string name = nameBox.Text.Trim().Replace(';', ' ');
        if (name == "")
        {
            MessageBox.Show("Please type a subject name.");
            return;
        }

        tracker.Add(new Subject(name, (double)gradeBox.Value, (double)coefBox.Value));
        nameBox.Clear();
        RefreshGrades();
        SaveAll();
    }

    private void UpdateButton_Click(object? sender, EventArgs e)
    {
        int index = SelectedGradeIndex();
        if (index < 0)
        {
            MessageBox.Show("Select a subject in the list first.");
            return;
        }

        tracker.Subjects[index].Grade = (double)gradeBox.Value;
        RefreshGrades();
        SaveAll();
    }

    private void RemoveButton_Click(object? sender, EventArgs e)
    {
        int index = SelectedGradeIndex();
        if (index < 0)
        {
            MessageBox.Show("Select a subject in the list first.");
            return;
        }

        tracker.Remove(index);
        RefreshGrades();
        SaveAll();
    }

    // ---------- Buildings tab ----------

    private TabPage BuildBuildingsTab()
    {
        var page = new TabPage("Buildings");

        // The list (fills the middle)
        buildingsList.Dock = DockStyle.Fill;
        buildingsList.View = View.Details;
        buildingsList.FullRowSelect = true;
        buildingsList.GridLines = true;
        buildingsList.MultiSelect = false;
        buildingsList.HideSelection = false;
        buildingsList.Columns.Add("Building", 140);
        buildingsList.Columns.Add("Owned", 70);
        buildingsList.Columns.Add("Cost", 220);
        buildingsList.Columns.Add("Sell for", 80);
        buildingsList.Columns.Add("Effect", 420);

        // Coins and resources at the top
        buildingsStatusLabel.Dock = DockStyle.Top;
        buildingsStatusLabel.Height = 40;
        buildingsStatusLabel.Padding = new Padding(10, 10, 0, 0);
        buildingsStatusLabel.Font = new Font(Font, FontStyle.Bold);

        // Buttons at the bottom
        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            AutoSize = true,
            Padding = new Padding(10)
        };

        var buildButton = new Button { Text = "Build selected", AutoSize = true };
        var sellButton = new Button { Text = "Sell selected", AutoSize = true };
        buildButton.Click += BuildButton_Click;
        sellButton.Click += SellButton_Click;
        buttons.Controls.Add(buildButton);
        buttons.Controls.Add(sellButton);

        // The middle control is added first, then the ones docked to the edges
        page.Controls.Add(buildingsList);
        page.Controls.Add(buttons);
        page.Controls.Add(buildingsStatusLabel);
        return page;
    }

    private static string CostText(Colony c, Building b)
    {
        string text = $"{c.GetPrice(b)} coins";
        Dictionary<ResourceType, int> resourceCost = c.GetResourceCost(b);

        if (resourceCost.Count > 0)
        {
            text += ", " + FormatResources(resourceCost);
        }

        return text;
    }

    private static string EffectText(Building b)
    {
        var effects = new List<string>();

        if (b.IncomeBonus > 0) effects.Add($"+{b.IncomeBonus} coins per turn");
        if (b.IncomePerSubject > 0) effects.Add($"+{b.IncomePerSubject} coins per subject");
        if (b.GradeIncomePercent > 0) effects.Add($"+{b.GradeIncomePercent}% of grade income");
        if (b.InterestPercent > 0) effects.Add($"{b.InterestPercent}% interest (max {Colony.MaxInterestPerBank} per turn)");
        if (b.Produces.Count > 0) effects.Add($"makes {FormatResources(b.Produces)}");
        if (b.Consumes.Count > 0) effects.Add($"uses {FormatResources(b.Consumes)}");

        return string.Join("; ", effects);
    }

    // Rebuilds the list and keeps the same row selected
    private void RefreshBuildings()
    {
        int selected = buildingsList.SelectedIndices.Count > 0 ? buildingsList.SelectedIndices[0] : -1;

        buildingsList.Items.Clear();

        foreach (Building b in Building.All)
        {
            int owned = colony.GetOwned(b);

            buildingsList.Items.Add(new ListViewItem(new[]
            {
                b.Name,
                owned.ToString(),
                CostText(colony, b),
                owned > 0 ? colony.GetRefund(b).ToString() : "-",
                EffectText(b)
            }));
        }

        if (selected >= 0 && selected < buildingsList.Items.Count)
        {
            buildingsList.Items[selected].Selected = true;
        }

        var resources = new List<string>();
        foreach (var pair in colony.Resources)
        {
            resources.Add($"{pair.Key} {pair.Value}");
        }
        buildingsStatusLabel.Text = $"Coins: {colony.Coins}   |   " + string.Join(", ", resources);
    }

    private void BuildButton_Click(object? sender, EventArgs e)
    {
        if (buildingsList.SelectedIndices.Count == 0)
        {
            MessageBox.Show("Select a building in the list first.");
            return;
        }

        Building chosen = Building.All[buildingsList.SelectedIndices[0]];

        if (!colony.TryBuild(chosen))
        {
            MessageBox.Show("Not enough coins or resources.");
            return;
        }

        RefreshStats();
        RefreshBuildings();
        SaveAll();
    }

    private void SellButton_Click(object? sender, EventArgs e)
    {
        if (buildingsList.SelectedIndices.Count == 0)
        {
            MessageBox.Show("Select a building in the list first.");
            return;
        }

        Building chosen = Building.All[buildingsList.SelectedIndices[0]];

        if (!colony.TrySell(chosen))
        {
            MessageBox.Show($"You don't own any {chosen.Name}.");
            return;
        }

        RefreshStats();
        RefreshBuildings();
        SaveAll();
    }

    // ---------- Map tab ----------

    private TabPage BuildMapTab()
    {
        var page = new TabPage("Map");

        // OpenStreetMap asks apps to identify themselves
        GMapProvider.UserAgent = "GradeColonyGame/1.0 (student project)";
        GMaps.Instance.Mode = AccessMode.ServerAndCache;

        map.Dock = DockStyle.Fill;
        map.MapProvider = GMapProviders.OpenStreetMap;
        map.MinZoom = 8;
        map.MaxZoom = 18;
        map.Zoom = 12;
        map.Position = new PointLatLng(36.8065, 10.1815);   // around the center of Tunis
        map.ShowCenter = false;
        map.DragButton = MouseButtons.Left;                  // drag with the left mouse button to move
        map.MouseWheelZoomType = MouseWheelZoomType.MousePositionWithoutCenter;   // wheel zooms in and out
        map.Overlays.Add(universityOverlay);

        int missing = 0;
        foreach (University u in University.All)
        {
            if (u.Latitude == null || u.Longitude == null)
            {
                missing++;
                continue;
            }

            var marker = new GMarkerGoogle(new PointLatLng(u.Latitude.Value, u.Longitude.Value), GMarkerGoogleType.red_dot)
            {
                ToolTipText = u.Name,
                ToolTipMode = MarkerTooltipMode.OnMouseOver,
                Tag = u
            };
            universityOverlay.Markers.Add(marker);
        }

        // Your own university: a different marker (green pin) with a label that is always visible.
        // It is added last so it is drawn on top of the red markers.
        var homeMarker = new GMarkerGoogle(new PointLatLng(HomeLatitude, HomeLongitude), GMarkerGoogleType.green_pushpin)
        {
            ToolTipText = $"{HomeName}{Environment.NewLine}YOUR UNIVERSITY",
            ToolTipMode = MarkerTooltipMode.Always,
            Tag = HomeTag
        };
        universityOverlay.Markers.Add(homeMarker);

        map.OnMarkerClick += Map_OnMarkerClick;
        map.MouseClick += Map_MouseClick;

        mapStatusLabel.Dock = DockStyle.Bottom;
        mapStatusLabel.Height = 60;
        mapStatusLabel.Padding = new Padding(10, 8, 0, 0);
        mapStatusLabel.Text = "The green pin is your university (UIK). Left-click a red marker to attack that university or view its info. Right-click anywhere to copy its coordinates."
            + (missing > 0 ? $"  ({missing} universities have no coordinates yet, so they are not on the map.)" : "");

        // The map is added first, then the label docked to the bottom
        page.Controls.Add(map);
        page.Controls.Add(mapStatusLabel);
        return page;
    }

    // Clicking a marker opens a small menu with two choices
    private void Map_OnMarkerClick(GMapMarker item, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left) return;

        // Your own university: show your colony instead of the attack menu
        if (item.Tag is string tag && tag == HomeTag)
        {
            ShowHomeDialog();
            return;
        }

        if (item.Tag is not University target) return;

        var menu = new ContextMenuStrip();
        menu.Items.Add(new ToolStripLabel(target.Name) { Font = new Font(Font, FontStyle.Bold) });
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Attack", null, (sender, args) => ShowAttackDialog(target));
        menu.Items.Add("View info", null, (sender, args) => ShowInfoDialog(target));
        menu.Show(Cursor.Position);
    }

    // Shows your own colony when you click the green pin
    private void ShowHomeDialog()
    {
        var lines = new List<string>
        {
            HomeName,
            "",
            $"Turn: {colony.Turn}",
            $"Coins: {colony.Coins}"
        };

        foreach (var pair in colony.Resources)
        {
            lines.Add($"{pair.Key}: {pair.Value}");
        }

        MessageBox.Show(string.Join(Environment.NewLine, lines), "Your university");
    }

    // Looks for images\<university name>.jpg (or .jpeg / .png) next to the program
    private static string? FindUniversityImage(University u)
    {
        string folder = Path.Combine(AppContext.BaseDirectory, "images");

        foreach (string extension in new[] { ".jpg", ".jpeg", ".png" })
        {
            string path = Path.Combine(folder, u.Name + extension);
            if (File.Exists(path)) return path;
        }

        return null;
    }

    private void ShowInfoDialog(University u)
    {
        using var dialog = new Form
        {
            Text = u.Name,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition = FormStartPosition.CenterParent,
            MaximizeBox = false,
            MinimizeBox = false,
            ClientSize = new Size(460, 540),
            Font = Font
        };

        // Picture (or a gray box that says where to put one)
        var picture = new PictureBox
        {
            Left = 20,
            Top = 20,
            Width = 420,
            Height = 260,
            SizeMode = PictureBoxSizeMode.Zoom,
            BackColor = Color.Gainsboro
        };

        string? imagePath = FindUniversityImage(u);
        if (imagePath != null)
        {
            try
            {
                // Copy the image so the file is not left locked
                using var stream = new MemoryStream(File.ReadAllBytes(imagePath));
                using var original = Image.FromStream(stream);
                picture.Image = new Bitmap(original);
            }
            catch (Exception)
            {
                imagePath = null;
            }
        }

        dialog.Controls.Add(picture);

        if (imagePath == null)
        {
            dialog.Controls.Add(new Label
            {
                Left = 30,
                Top = 110,
                Width = 400,
                Height = 80,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Gainsboro,
                ForeColor = Color.DimGray,
                Text = $"No picture yet.{Environment.NewLine}Save one as images\\{u.Name}.jpg{Environment.NewLine}next to the program."
            });
            dialog.Controls.SetChildIndex(dialog.Controls[dialog.Controls.Count - 1], 0);
        }

        // Name and numbers
        var lines = new List<string> { u.Name, "", $"Coins: {u.Coins}" };
        foreach (var pair in u.Resources)
        {
            lines.Add($"{pair.Key}: {pair.Value}");
        }

        var info = new Label
        {
            AutoSize = true,
            Left = 20,
            Top = 300,
            MaximumSize = new Size(420, 0),
            Text = string.Join(Environment.NewLine, lines)
        };
        dialog.Controls.Add(info);

        var closeButton = new Button { Text = "Close", Left = 350, Top = 490, Width = 90, DialogResult = DialogResult.OK };
        dialog.Controls.Add(closeButton);
        dialog.AcceptButton = closeButton;
        dialog.CancelButton = closeButton;

        dialog.FormClosed += (sender, args) => picture.Image?.Dispose();
        dialog.ShowDialog(this);
    }

    // Right-click copies the coordinates of that spot, so you can paste them into Universities.cs
    private void Map_MouseClick(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Right) return;

        PointLatLng point = map.FromLocalToLatLng(e.X, e.Y);
        string text = FormattableString.Invariant($"Latitude = {point.Lat:0.000000}, Longitude = {point.Lng:0.000000}");

        try
        {
            Clipboard.SetText(text);
            mapStatusLabel.Text = $"Copied: {text}";
        }
        catch (Exception)
        {
            mapStatusLabel.Text = text;
        }
    }

    private void ShowAttackDialog(University target)
    {
        int available = colony.Resources[ResourceType.Soldiers];
        int defenders = target.Resources[ResourceType.Soldiers];

        if (available <= 0)
        {
            MessageBox.Show(
                $"{target.Name} has {defenders} soldiers.{Environment.NewLine}{Environment.NewLine}" +
                "You have no soldiers. Build Barracks and end a few turns first.",
                target.Name);
            return;
        }

        using var dialog = new Form
        {
            Text = target.Name,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition = FormStartPosition.CenterParent,
            MaximizeBox = false,
            MinimizeBox = false,
            ClientSize = new Size(400, 200),
            Font = Font
        };

        var info = new Label
        {
            AutoSize = true,
            Left = 20,
            Top = 15,
            Text = $"{target.Name}{Environment.NewLine}" +
                   $"Soldiers defending: {defenders}{Environment.NewLine}" +
                   $"You have {available} soldiers."
        };

        var sendLabel = new Label { AutoSize = true, Left = 20, Top = 100, Text = "Soldiers to send:" };

        var sendBox = new NumericUpDown
        {
            Left = 190,
            Top = 97,
            Width = 90,
            Minimum = 1,
            Maximum = available,
            Value = Math.Min(available, Math.Max(1, defenders))
        };

        var attackButton = new Button { Text = "Attack", Left = 190, Top = 145, Width = 90, DialogResult = DialogResult.OK };
        var cancelButton = new Button { Text = "Cancel", Left = 290, Top = 145, Width = 90, DialogResult = DialogResult.Cancel };

        dialog.Controls.Add(info);
        dialog.Controls.Add(sendLabel);
        dialog.Controls.Add(sendBox);
        dialog.Controls.Add(attackButton);
        dialog.Controls.Add(cancelButton);
        dialog.AcceptButton = attackButton;
        dialog.CancelButton = cancelButton;

        if (dialog.ShowDialog(this) != DialogResult.OK) return;

        BattleResult? result = Battle.Attack(colony, target, (int)sendBox.Value);
        if (result == null) return;

        string message = result.Won
            ? $"VICTORY against {result.TargetName}!"
            : $"DEFEAT. {result.TargetName} held their ground.";

        message += $"{Environment.NewLine}{Environment.NewLine}You lost {result.AttackerLosses} soldiers. They lost {result.DefenderLosses}.";

        if (result.Won)
        {
            message += $"{Environment.NewLine}Loot: {result.LootCoins} coins, {FormatResources(result.LootResources)}";
        }

        MessageBox.Show(message, target.Name);

        RefreshStats();
        RefreshBuildings();
        SaveAll();
    }
}