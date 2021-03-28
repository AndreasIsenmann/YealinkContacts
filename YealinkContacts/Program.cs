using System;
using System.IO;
using System.Text;
using System.Drawing;

namespace YealinkContacts
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("YealinkContacts V{0}", typeof(MainClass).Assembly.GetName().Version);
                Console.WriteLine("====================================");
                Console.WriteLine("(c) 2021 Andreas Isenmann");
                Console.WriteLine("Usage: YealinkContacts <vCardFilname> <OutputFilename>");
                return;
            }

            try
            {
                ProcessInputFile(args[0], args[1]);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("File not found: {0}", e.Message);
            }
        }

        private static void ProcessInputFile(string fnInput, string fnOutput)
        {
            string line;
            YealinkContact yc = null;
            StreamReader sr = new StreamReader(fnInput);
            StreamWriter sw = new StreamWriter(fnOutput);

            WriteBeginning(sw);

            sw.WriteLine("<root_contact>");
            while (null != (line = sr.ReadLine()))
            {
                if (null == yc)
                {
                    yc = new YealinkContact();
                }

                if (line.StartsWith("PHOTO;"))
                {
                    line = ExtractImage(line.Substring(6), sr, yc);
                }


                if (line.StartsWith("N:"))
                {
                    ExtractName(line.Substring(2), yc);
                }

                if (line.StartsWith("TEL;"))
                {
                    ExtractPhoneNumbers(line.Substring(4), yc);
                }

                if (line.StartsWith("END:VCARD"))
                {
                    var xml = yc.GetXmlString();
                    if (xml != string.Empty)
                    {
                        sw.WriteLine(xml);
                    }

                    yc = null;
                }

            }
            sw.WriteLine("<root_contact>");
        }

        private static void WriteBeginning(StreamWriter sw)
        {
            sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            sw.WriteLine("<root_group>");
            sw.WriteLine("<group display_name=\"All Contacts\" />");
            sw.WriteLine("<group display_name=\"Blacklist\" />");
            sw.WriteLine("</root_group>");
        }

        private static void ExtractName(string line, YealinkContact yc)
        {
            var info = line.Split(';');
            yc.FullName = info[0] + " " + info[1];
        }

        private static void ExtractPhoneNumbers(string line, YealinkContact yc)
        {
            if (line.IndexOf("type=CELL;") >= 0)
            {
                yc.Mobile = line.Substring(line.LastIndexOf(":") + 1);
            }
            else if (line.IndexOf("type=HOME;") >= 0 || line.IndexOf("type=OTHER;") >= 0)
            {
                yc.PhoneHome = line.Substring(line.LastIndexOf(":") + 1);
            }
            else if (line.IndexOf("type=WORK;") >= 0 || (line.IndexOf("type=MAIN;") >= 0 && yc.PhoneOffice == string.Empty))
            {
                yc.PhoneOffice = line.Substring(line.LastIndexOf(":") + 1);
            }
            else
            {

            }
        }

        private static void CreateImage(string imageBase64, string fileName)
        {
            byte[] imageData = Convert.FromBase64String(imageBase64);

            var byteStream = new MemoryStream(imageData);

            FileStream fs = new FileStream(fileName, FileMode.Create);
            BinaryWriter br = new BinaryWriter(fs);
            br.Write(imageData);
            br.Close();
            fs.Close();
        }

        static int imageNumber = 0;
        private static string ExtractImage(string first, StreamReader sr, YealinkContact yc)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(first.Substring(first.LastIndexOf(":") + 1));

            string line;
            while (null != (line = sr.ReadLine()))
            {
                if(line.StartsWith(" "))
                {
                    sb.Append(line.Trim());
                }
                else
                {
                    string imageName;
                    if(yc.FullName!=string.Empty)
                    {
                        imageName = yc.FullName.Replace(" ","_") + ".jpg";
                    }
                    else
                    {
                        imageName = string.Format("ContactImage{0}.jpg", imageNumber++);
                    }
                    CreateImage(sb.ToString(), imageName);
                    break;
                }

            }

            return line;
        }
    }
}
