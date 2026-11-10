float onescore, secondscore, thirdscore, averagescore;
Console.WriteLine("Enter the first score:");
onescore = float.Parse(Console.ReadLine());
Console.WriteLine("Enter the second score:");
secondscore= float.Parse(Console.ReadLine());
Console.WriteLine("Enter the third score:");
thirdscore = float.Parse(Console.ReadLine());
averagescore = (onescore + secondscore + thirdscore) / 3;
Console.WriteLine("The average score is:" + averagescore);
