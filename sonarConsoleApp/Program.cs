using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sonarConsoleApp
{
    class Program
    {
        const string fileName = "R00063.DAT";
        const string pathName = "F:\\coding\\proj\\sonarViewer\\data\\";

        static void Main(string[] args)
        {
            //Read DAT file contents into a single byte array
            byte[] datFileArray = File.ReadAllBytes(pathName + fileName);

            //Check frame header value of $C1
            int HeaderValue = datFileArray[0];
            if ( HeaderValue != 193 )
            {
                Console.WriteLine("Bad header detected terminating program") ;
            }
            else
            {
                Console.WriteLine("Good header detected");
            }

            //Extract Water Type Application
            int WaterTypeUse = datFileArray[1];
            Console.WriteLine("Water Usage Type: {0}" , WaterTypeUse);

            //Sonar Model Type
            int SonarModel = datFileArray[7];
            Console.WriteLine("Sonar Type: {0}", SonarModel);

            //Display Date and Time of file creation
            // time is in unsigned int32 format and big-endian format
            //UInt32   InitUnixTime = BitConverter.ToUInt32(Array.Reverse(datFileArray,11)) ;
            
            byte[] UnixTimeBytes = new byte[4];
            Array.Copy(datFileArray, 20, UnixTimeBytes, 0, 4);
            Array.Reverse(UnixTimeBytes);
            UInt32 InitUnixTime = BitConverter.ToUInt32(UnixTimeBytes, 0);
            //Console.WriteLine("unix time value: {0}", InitUnixTime);
            DateTime startDateTime = UnixTimestampToDateTime(InitUnixTime);
            Console.WriteLine(startDateTime.ToString("R")); 
                    
            //Extract init Geo-spatial values from file
            //LON
            byte[] LonBytes = new byte[4];
            Array.Copy(datFileArray, 24, LonBytes, 0, 4);
            Array.Reverse(LonBytes);
            Int32 LonDec = BitConverter.ToInt32(LonBytes, 0);
            Console.WriteLine("lon dec value: {0}", LonDec);
            //Console.ReadLine();

            //LAT
            byte[] LatBytes = new byte[4];
            Array.Copy(datFileArray, 28, LatBytes, 0, 4);
            Array.Reverse(LatBytes);
            Int32 LatDec = BitConverter.ToInt32(LatBytes, 0);
            Console.WriteLine("lat dec value: {0}", LatDec);

            //Convert Mercator Meter values into Geo-spatial LON LAT decimal values
            double Lon = MercatorMetersToGeo( LatDec, LonDec);
            Console.WriteLine("lat value: {0}", Lon);
            Console.ReadLine();
            

            ShowArrayInfo(datFileArray);
            Console.ReadLine();
            // BinaryReader datFileBytes = new BinaryReader(File.OpenRead(pathName+fileName));

            foreach (int datValue in datFileArray)
                Console.WriteLine(datValue);
            Console.ReadLine();
        }

        private static void ShowArrayInfo(Array datFileArray)
        {
            Console.WriteLine("Length of DAT File Array:--> {0}", datFileArray.Length);
            Console.WriteLine();
        }

        /// <summary>
        /// Methods to convert Unix time stamp to DateTime
        /// </summary>
        /// <param name="_UnixTimeStamp">Unix time stamp to convert windows time</param>
        /// <returns>Return DateTime</returns>
        public static DateTime    UnixTimestampToDateTime(UInt32 _UnixTimestamp)
        {
            return (new DateTime(1970, 1, 1, 0, 0, 0)).AddSeconds(_UnixTimestamp);
        }

        /// <summary>
        /// Methods to convert Mercaor Meters to LON and LAT decimal values
        /// </summary>
        /// <param name="LatMm abd LonMm">Lat and Lon in Mercator Meters to be converted</param>
        /// <returns>Return DateTime</returns>
        public static double MercatorMetersToGeo(double LatMm, double LonMm) 
        {
        /* Local variable declarations and assignements */
        const double semiMajorAxis = 6378388 ;
        const double alpha = 0.003367003 ;
        //const double mercatorMeterLatMax = 23500000 ;
        //const double mercatorMeterLonMax = 20500000 ;

        /* Computed constants */
        double pi = Math.PI ;
        double radians2Degrees = ( 360 / (2 * pi ) ) ;
        double e2 = 2*alpha - Math.Pow(alpha,2) ;
        double k2 = (e2 / 2) + (5 / 24) * Math.Pow(e2,2) + (Math.Pow(e2,3) / 12) ;
        double k4 = (7 / 48) * Math.Pow(e2,2) + (29 / 240) * Math.Pow(e2,3) ;
        double k6 = (7 / 120) * Math.Pow(e2,3) ;

        /* Conversion algorithm calculations */
        double phi = ( 2 * Math.Atan( Math.Exp( LatMm /semiMajorAxis ) ) ) - ( pi / 2 ) ;
        double geoLat = ( phi + k2 * Math.Sin( 2 * phi ) + k4 * Math.Sin( 4 * phi ) + k6 * Math.Sin( 6 * phi ) ) * radians2Degrees ;
        double geoLon = ( LonMm / semiMajorAxis ) * radians2Degrees ;

        return geoLon ;
        
        }
 
    }
}
