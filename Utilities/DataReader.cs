namespace Analysis_Lab1.Utilities
{
    internal static class DataReader
    {
        internal static List<double>? LoadDataFromFile(string fileName)
        {
            if (!File.Exists(fileName)) 
            {
                Console.WriteLine("Files does not exist.");
                return null;
            }

            List<double> values = new List<double>();

            try
            {
                string[] lines = File.ReadAllLines(fileName);

                foreach(string line in lines)
                {
                    string[] numbers = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    foreach(string number in numbers)
                    {
                        if (double.TryParse(number, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double parsedValue))
                        {
                            values.Add(parsedValue);
                        }
                        else
                        {
                            Console.WriteLine($"Error occured when parsing {number}.");
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error loading data from file: {ex.Message}");
                return null;
            }

            return values;
        }
    }
}
