/*
AUTHOR
Andrew Green
acgreen@ucdavis.edu

DATE
1/28/2014

SUMMARY
Simple script which takes in an input.txt and generates an output.txt
Just copy the output (ctrl+a, ctrl+c) then go to logisim and paste it into the combinational analysis
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace OutputCountingNumbers
{
	class Program
	{
		static void Main(string[] args)
		{
			//Make sure they have an input file
			if (!File.Exists("input.txt"))
			{
				Console.WriteLine("input.txt not found! Make sure that it's in the same directory as this exe then run again.\nHit any key to exit this program.");
				Console.ReadKey();
				return;
			}

			//Track longest input so we know how many bits each field is
			int input_bit_length = 0;
			int output_bit_length = 0;

			var write_values = new List<Tuple<int, string>>();

			using (StreamReader read = new StreamReader("input.txt"))
			{
				//Read input line
				string line;
				while ((line = read.ReadLine()) != null)
				{
					string[] values = line.Split(' ');   //try spaces
					if (values.Length < 2)
						values = line.Split('\t');   //try tabs
					if (values.Length == 1)	//give up
					{
						Console.WriteLine("Your input seems screwy, you've got just one value on a line! Fix that then run again.\nHit any key to exit this program.");
						Console.ReadKey();
						return;
					}

					input_bit_length = Math.Max(input_bit_length, values[0].Length);
					output_bit_length = Math.Max(output_bit_length, values[1].Length);

					//Begin converting 'x' into either 0 or 1
					List<string> convert_to_int = new List<string>();
					convert_to_int.Add(values[0]);

					while (convert_to_int.Count != 0)
					{
						//Use first item
						string convert_me = convert_to_int[0];
						convert_to_int.RemoveAt(0);

						bool found_x = false;
						for (int i = 0; i < convert_me.Length; i++)
						{
							//If there's an x or X replace with two strings, one with 0 and another with 1
							if (convert_me[i] == 'x' || convert_me[i] == 'X')
							{
								char[] copy = convert_me.ToCharArray();
								copy[i] = '0';
								convert_to_int.Add(new string(copy));
								copy[i] = '1';
								convert_to_int.Add(new string(copy));
								found_x = true;
								break;
							}
						}

						//String had no x so add to write list
						if (!found_x)
							write_values.Add(new Tuple<int, string>(
								Convert.ToInt32(convert_me, 2),
								values[1]));
					}
				}

				int output_line_count = (int)Math.Round(Math.Pow(2, input_bit_length));	//calculate length of file
				using (StreamWriter file = new StreamWriter("output.txt"))
				{
					//Write each line as zeroes or as input
					for (int i = 0; i < output_line_count; i++)
					{
						Tuple<int, string> matching_tuple = write_values.Find(item => item.Item1 == i);
						string write_string = matching_tuple == null ? "" : matching_tuple.Item2;
						write_string = write_string.PadLeft(output_bit_length, '0');

						foreach (char c in write_string)
							file.Write(c + "\t");   //logisim wants tab deliniated
						file.WriteLine();
					}
				}

				//Finish up
				Process.Start("output.txt");
				Console.WriteLine(String.Join(Environment.NewLine,
					"Looks good here's some steps for how to use:",
					"	1. Copy all the text in output.txt (ctrl+a, ctrl+c)",
					"	2. Open logisim's combinatorial analysis",
					"	3. Select top left item in combinatorial analysis",
					"	4. Paste (ctrl+v)",
					"Hit any key to exit this program."));
				Console.ReadKey();
			}
		}
	}
}