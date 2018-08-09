using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Euresys.Open_eVision_2_5;
using System.Drawing;
using System.IO;

namespace ConsoleApp1
{
    class Program
    {
        static EImageC24 imageSource;
        static EImageBW8 imageSourceGray;

        //static EPatternFinder EPatternFinder1 = new EPatternFinder();
        static EMatcher EMatcher1 = new EMatcher();
        static EMatcher EMatcher2 = new EMatcher();

        static DirectoryInfo di = new DirectoryInfo(@"C:\Users\user\Downloads\IPS產品評估 (御杰) (002)\IPS產品評估 (御杰)");

        static void Main(string[] args)
        {

            int roiWidth1 = 58;
            int roiHeight1 = 39;
            int roiWidth2 = 22;
            int roiHeight2 = 130;
            imageSource = new EImageC24();
            imageSource.SetSize(768, 576);//測試其他圖之前先修改圖這邊的影像尺寸，影像尺寸須與原圖相同

            //EPatternFinder1.MaxInstances = 40;
            //EPatternFinder1.MinScore = 0.8f;

            imageSourceGray = new EImageBW8(imageSource.Width, imageSource.Height);
            EImageBW8 imgBrushed = new EImageBW8(imageSourceGray);
            
            //載入樣板
            //EPatternFinder1.Load(Path.Combine(di.FullName, "EasyFindTemplate1.FND"));
            EMatcher1.Load(Path.Combine(di.FullName, "EMatcherTemplate1.MCH"));
            EMatcher2.Load(Path.Combine(di.FullName, "EMatcherTemplate2.MCH"));

            FileInfo[] files = di.GetFiles("*.jpg");
            foreach (var file in files)
            {
                imageSource.Load(Path.Combine(di.FullName, file.Name));
                EasyColor.C24ToBayer(imageSource, imageSourceGray);
                EasyImage.Copy(imageSourceGray, imgBrushed);

                //Find
                //EFoundPattern[] founds = EPatternFinder1.Find(imageSourceGray);
                EMatcher1.Match(imageSourceGray);
                EMatcher2.Match(imageSourceGray);

                IntPtr gintptr = Easy.OpenImageGraphicContext(imgBrushed);
                Graphics g = Graphics.FromHdc(gintptr);
                //foreach (var item in founds)
                //{
                //    EPoint center = item.Center;

                //    //g.FillRectangle(new SolidBrush(Color.White)
                //    //            , (int)center.X - ((int)roiWidth / 2), (int)center.Y - ((int)roiHeight / 2)
                //    //            , roiWidth, roiHeight);

                //    item.Draw(g, new ERGBColor(255, 255, 255), 1.0f, 1.0f, 0, 0);
                //}

                foreach (var item in EMatcher1.Positions)
                {
                    EPoint center = new EPoint(item.CenterX, item.CenterY);
                    g.FillRectangle(new SolidBrush(Color.White)
                                , (int)center.X - ((int)roiWidth1 / 2), (int)center.Y - ((int)roiHeight1 / 2)
                                , roiWidth1, roiHeight1);
                }

                foreach (var item in EMatcher2.Positions)
                {
                    EPoint center = new EPoint(item.CenterX, item.CenterY);
                    g.FillRectangle(new SolidBrush(Color.White)
                                , (int)center.X - ((int)roiWidth2 / 2), (int)center.Y - ((int)roiHeight2 / 2)
                                , roiWidth2, roiHeight2);
                }

                Easy.CloseImageGraphicContext(imgBrushed, gintptr);

                imgBrushed.SaveJpeg(@"D:\" + file.Name);
            }
        }
    }
}
