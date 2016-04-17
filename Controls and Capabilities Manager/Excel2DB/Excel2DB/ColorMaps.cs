using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excel2DB
{
    class ColorMaps
    {

        public static SortedList<uint, int[]> CIA;
        public static SortedList<uint, int[]> CIASeparate;
        public static SortedList<string, double> valueassign;
        public static SortedList<double, int[]> responsibility;
        public static void colorAll()
        {
            CIA = new SortedList<uint, int[]>();
            CIASeparate = new SortedList<uint, int[]>();
            CIA.Add(0, new int[] { 255, 255, 255 });
            CIA.Add(1, new int[] { 31, 71, 126 });
            CIA.Add(2, new int[] { 49, 112, 196 });
            CIA.Add(3, new int[] { 84, 139, 212 });
            CIA.Add(4, new int[] { 0, 155, 194 });
            CIA.Add(5, new int[] { 63, 192, 164 });
            CIA.Add(6, new int[] { 98, 180, 54 });
            CIA.Add(7, new int[] { 171, 203, 40 });
            CIA.Add(8, new int[] { 248, 226, 1 });
            CIA.Add(9, new int[] { 252, 190, 17 });
            CIA.Add(10, new int[] { 240, 129, 24 });
            CIA.Add(11, new int[] { 238, 87, 30 });
            CIA.Add(12, new int[] { 235, 29, 29 });

            CIASeparate.Add(0, new int[] { 255, 255, 255 });
            CIASeparate.Add(1, new int[] { 98, 180, 154 });
            CIASeparate.Add(2, new int[] { 248, 226, 1 });
            CIASeparate.Add(3, new int[] { 240, 129, 24 });
            CIASeparate.Add(4, new int[] { 235, 29, 29 });

            valueassign = new SortedList<string, double>();
            valueassign.Add("AX,AX,AX", 3);
            valueassign.Add("XX,AX,AX", 2.5);
            valueassign.Add("XA,AX,AX", 2);
            valueassign.Add("XX,XX,AX", 2);
            valueassign.Add("XA,XX,AX", 1.5);
            valueassign.Add("XX,XX,XX", 1.5);
            valueassign.Add("XA,XA,AX", 1);
            valueassign.Add("XA,XX,XX", 1);
            valueassign.Add("XA,XA,XX", 0.5);
            valueassign.Add("XA,XA,XA", 0);
            valueassign.Add("AA,AA,AA", -1);

            responsibility = new SortedList<double, int[]>();
            responsibility.Add( 3,new int[]{255,20,20});
            responsibility.Add(2.5,new int[]{255,140,0});
            responsibility.Add( 2,new int[]{255,246,0});
            responsibility.Add(1.5,new int[]{128,225,17});
            responsibility.Add( 1,new int[]{0,185,88});
            responsibility.Add(.5,new int[]{0,252,252});
            responsibility.Add( 0,new int[]{82,147,238});
            responsibility.Add(-1,new int[]{4,34,255});
            responsibility.Add(-2,new int[]{100,100,100});

        }
    }
}
