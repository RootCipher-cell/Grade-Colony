
namespace test
{
    

    public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
    }

    private void button1_Click(object sender, EventArgs e)
    {
        string Rami = "Rami";
            
        if (textBox1.Text == "Rami")
        {
            MessageBox.Show("You areRami");
        }
        else
        {
            MessageBox.Show("You are not Rami");
        }
    }
        
        
           private void textBox2_TextChanged(object sender, EventArgs e)
           {
           }


    

    }

   


}


