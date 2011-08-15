namespace ZIndexing.RnaStruct
{
    public class RNA
    {
        public static bool IsCompatible(char c1, char c2)
        {
            switch (c1)
            {
                case 'G':
                    return c2 == 'U' || c2 == 'C';
                case 'A':
                    return c2 == 'U';
                case 'C':
                    return c2 == 'G';
                case 'U':
                    return c2 == 'A' || c2 == 'G';
            }

            return false;
        }
    }
}