using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ADVCS
{
    struct pt
    {
        public byte x, y;
    }

    struct obj
    {
        public pt               pos;
        public byte             flags;
        public byte[]           spr;
        public ConsoleColor     col;
    }

    struct room
    {

    }

    class prg
    {
        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        // 0b10000000 = entity, 0b00000000 = item
        const  byte w = 160, h = 96, sprw = 16;
        static byte px = 32, py = 32, pxp, pyp, src = 0;
        static ConsoleColor pfcolor = ConsoleColor.Magenta;
        static int logic = 0, rate = 36;
        static List<pt> newPx = new List<pt>(), owPFPx = new List<pt>();
        static Random trophy = new Random();
        static obj[] objects = new obj[1];

        static void draw(object o)
        {
            if (newPx.Count > 0)
                frame();
            //Console.Clear();
            //fun V
            /*Console.ForegroundColor = ConsoleColor.Green;
            spr(sprdragondead, (byte)(px - 48), py, true);
            Console.ForegroundColor = ConsoleColor.Yellow;
            spr(sprdragonbite, (byte)(px - 32), py, false);
            Console.ForegroundColor = ConsoleColor.Red;
            spr(sprdragon, (byte)(px - 16), py, false);
            Console.ForegroundColor = ConsoleColor.White;
            spr(sprkey, (byte)(px + 7), py, false);
            Console.ForegroundColor = ConsoleColor.Black;
            spr(sprmag, (byte)(px + 18), py, false);
            Console.ForegroundColor = ConsoleColor.Yellow;
            spr(sprswrd, (byte)(px + 28), py, false);
            Console.ForegroundColor = (ConsoleColor)trophy.Next(0, 16);
            spr(sprtro, (byte)(px + 38), py, false);*/
            for (byte i = 0; i < objects.Length; i++)
            {
                Console.BackgroundColor = objects[i].col;
                spr(objects[i].spr,
                    objects[i].pos.x,
                    objects[i].pos.y, false);
            }
            rect(px, py, 4, 4, pfcolor);
            Console.SetCursorPosition(0, 0);
            new Timer(draw, null, rate, 0);
        }

        static void nPx(byte x, byte y)
        {
            pt newp;
            newp.x = x;
            newp.y = y;
            newPx.Add(newp);
        }

        [MTAThread]
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.Title = "Adventure";
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Clear();
                Console.SetWindowSize(44, 27);
                Console.SetBufferSize(44, 27);
                Console.Write("\n\n\n\n      \"Adventure\"\n\n" +
    @"   An evil magician has stolen the
   Enchanted Chalic and has hidden it
   somewhere in the kingdom.  The object
   of the game is to rescue the Enchanted
   Chalice and place it inside the Golden
   Castle where it belongs. But, beware,
   this is no easy task as the magician
   has created 3 dragons to hinder you
   in your quest for the Enchanted
   Chalice.

   PC port by donnaken15

   Controls..

   Select               . Game Mode
   Left.Right.Up.Down   . Move Player
   Fire                 . Place Bridge
   Left,Right Select    . Diff. A off,on
   Up,Down Select       . Diff. B off,on

                     Press FIRE to START...");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Black;
                Console.ReadLine();
            }
            IntPtr handle = GetConsoleWindow();
            IntPtr sysMenu = GetSystemMenu(handle, false);
            DeleteMenu(sysMenu, 0xF030, 0);
            DeleteMenu(sysMenu, 0xF000, 0);
            Console.SetWindowSize(4, 4);
            Console.SetBufferSize(w+1, h+1);
            Console.SetWindowSize(w+1, h+1);
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.Clear();
            objects[0].pos.y = 40;
            objects[0].col = ConsoleColor.Red;
            objects[0].spr = sprdragon;
            pf(pfhallway, ConsoleColor.Magenta, false, false);
            //Console.OutputEncoding = Encoding.GetEncoding(437);
            draw(null);
            while (true)
            {
                /*if (px >= w - 8 && py >= h - 8)
                {
                    py = pyp;
                    px = pxp;
                }*/
                pyp = py;
                pxp = px;
                objects[0].pos.x += 1;
                if (GetForegroundWindow() == handle)
                {
                    if (GetAsyncKeyState(0x26) != 0)
                        py -= 4;
                    if (GetAsyncKeyState(0x28) != 0)
                        py += 4;
                    if (GetAsyncKeyState(0x25) != 0)
                        px -= 4;
                    if (GetAsyncKeyState(0x27) != 0)
                        px += 4;
                    if (GetAsyncKeyState(0x20) != 0)
                    {
                        //Console.Beep(160, 80);
                        //Console.Beep(333, 80);
                        Console.Beep(1000, 80);
                    }
                }
                Thread.Sleep(rate);
                /*switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.UpArrow:
                        py--;
                        break;
                    case ConsoleKey.DownArrow:
                        py++;
                        break;
                    case ConsoleKey.LeftArrow:
                        px--;
                        break;
                    case ConsoleKey.RightArrow:
                        px++;
                        break;
                }*/
            }
        }

        static byte flip(byte i)
        {
            int j = 0;//, k = 1;
            /*for (byte l = 0, m = 16; l < 4; l++)
            {
                j |= (i & k) << (7 - (l * 2));
                j |= (i & m) << (1 + (l * 2));
                k *= 2;
                m *= 2;
            }*/
            // absolutely wasteful, fix above later
            // ^ fixed on my tower, didnt push b/c idot cant code
            j |= (i & 1) << 7;
            j |= (i & 2) << 5;
            j |= (i & 4) << 3;
            j |= (i & 8) << 1;
            j |= (i & 16) >> 1;
            j |= (i & 32) >> 3;
            j |= (i & 64) >> 5;
            j |= (i & 128) >> 7;
            return (byte)(j & 0xFF);
        }

        static void rect(byte x, byte y, byte w, byte h, ConsoleColor col)
        {
            if (x >= 0 && y >= 0 && x < 158 && y < 96)
            {
                Console.BackgroundColor = col;
                //Console.ForegroundColor = col;
                byte j = 0;
                if (x >= w + 158)
                    j = (byte)(prg.w - x - 4);
                else
                    j = w;
                string block = new string('\0', j); //'\u2588'

                for (byte k = 0; k < h; k++)
                {
                    for (byte l = 0; l < w; l++)
                    {
                        nPx((byte)(x + l), (byte)(y + k));
                    }
                }

                for (byte i = 0; i < h && i < prg.h - y; i++)
                {
                    if (x >= 0 && x < 158 && y >= 0 && y < 96)
                    {
                        Console.SetCursorPosition(x, y + i);
                        Console.Write(block);
                    }
                }
            }
        }

        static void frame()
        {
            for (ushort i = 0; i < newPx.Count; i++)
                if (newPx[i].x >= 0 && newPx[i].x < 160 && newPx[i].y >= 0 && newPx[i].y < 96)
                {
                    pt temp = new pt();
                    temp.x = newPx[i].x;
                    temp.x = newPx[i].y;
                    owPFPx.Add(temp);
                    if (newPx[i].x == owPFPx.IndexOf(temp))
                    {
                        Console.BackgroundColor = pfcolor;
                    }
                    else
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.SetCursorPosition(newPx[i].x, newPx[i].y);
                    Console.Write('\0');
                }
            newPx.Clear();
        }

        static void spr(byte[] sprite, byte x, byte y, bool flip)
        {
            if (x >= 0 && y >= 0 && x < 158 && y < 96)
            {
                int ox, oy;
                ox = Console.CursorLeft;
                oy = Console.CursorTop;

                byte dots;

                for (byte i = 0; i < sprite.Length && i < h - y; i++)
                {
                    if (y >= 0 && y <= 96)
                    {
                        Console.SetCursorPosition(x, y + i);
                        dots = sprite[i];
                        if (flip)
                            dots = prg.flip(dots);
                        for (byte j = 0; j < 8 && j < w - x; j++)
                        {
                            string temp = string.Empty;
                            if ((dots & 1) == 1)
                            {
                                //Console.Write('\u2588');
                                temp += '\0';
                                nPx((byte)(x + j), (byte)(y + i));
                            }
                            else
                            {
                                if (j < w - x - 1)
                                    Console.CursorLeft++;
                            }
                            Console.Write(temp);
                            dots = (byte)(dots >> 1);
                        }
                    }
                }

                Console.SetCursorPosition(ox, oy);
            }

        }

        static void pf(byte[] field, ConsoleColor col, bool dblW, bool mirror)
        {
            Console.SetCursorPosition(0, 0);
            //seriously optimize this later
            pfcolor = col;
            Console.BackgroundColor = pfcolor;
            byte m = 16, n, o, p;
            byte[] blocks = new byte[3];
            for (byte i = 0; i < 19; i += 3)
            {
                blocks[0] = flip(field[i]);
                blocks[1] = flip(field[i + 1]);
                blocks[2] = flip(field[i + 2]);
                for (byte j = 0; j < 3; j++)
                {
                    for (byte k = 0; k < 8; k++)
                    {
                        n = (byte)Console.CursorLeft;
                        o = (byte)Console.CursorTop;
                        if ((blocks[j] & 1) != 1)
                            goto pfemptyblock;
                        p = m;
                        if (i == 0 || i == 18)
                        {
                            p /= 2;
                        }
                        for (byte l = 0; l < p; l++)
                        {
                            for (int q = 0; q < 8; q++)
                            {
                                pt temp = new pt();
                                temp.x = (byte)(n + q);
                                temp.x = o;
                                owPFPx.Add(temp);
                            }
                            Console.Write(new string('\0', m / 2));
                            try
                            {
                                if (i == 0 || i == 18)
                                {
                                    Console.CursorLeft = n;
                                    //Console.CursorLeft += m / 4;
                                    Console.CursorTop = o + 1 + l;
                                }
                                else
                                {
                                    Console.CursorLeft -= m / 2;
                                    Console.CursorTop++;
                                }
                            }
                            catch { }
                        }
                        pfemptyblock:
                        Console.CursorTop = o;
                        Console.CursorLeft += m / 2;
                        //Console.CursorTop -= 1;
                        //Console.Write("\0");
                        //Console.Write('\0');
                        blocks[j] = (byte)(blocks[j] >> 1);
                        if (j == 2 && k == 3)
                            goto pfnextline;
                    }
                }
            pfnextline:
                if (i == 0 || i == 18)
                    Console.CursorTop += m / 2 - 1;
                else
                    Console.CursorTop += m - 1;
                Console.WriteLine();
            }
            Console.SetCursorPosition(0, 0);
        }

        static byte[]
        #region sprites
            sprdragon =
        {
            0x06, 0x0F, 0xF3, 0xFE, 0x0E, 0x04, 0x04, 0x1E,
            0x3F, 0x7F, 0xE3, 0xC3, 0xC3, 0xC7, 0xFF, 0x3C,
            0x08, 0x8F, 0xE1, 0x3F
        },
            sprdragonbite = {
            0x80, 0x40, 0x26, 0x1F, 0x0B, 0x0E, 0x1E, 0x24,
            0x44, 0x8E, 0x1E, 0x3F, 0x7F, 0x7F, 0x7F, 0x7F,
            0x3E, 0x1C, 0x08, 0xF8, 0x80, 0xE0
        },
            sprdragondead = {
            0x0C, 0x0C, 0x0C, 0x0E, 0x1B, 0x7F, 0xCE, 0x80,
            0xFC, 0xFE, 0xFE, 0x7E, 0x78, 0x20, 0x6E, 0x42, 0x7E
        },
            sprkey = { 0x07, 0xFD, 0xA7 },
            sprswrd = { 32, 64, 255, 64, 32 },
            sprmag = { 60, 126, 231, 195, 195, 195, 195, 195 },
            sprtro = { 129, 129, 195, 126, 126, 60, 24, 24, 126 },
            sprbat1 = { 1, 128, 1, 128, 60, 90, 102, 195, 129, 129, 129 },
            sprbat2 = { 129, 129, 195, 195, 255, 90, 102 },
            sprent = { 0xF8, 0xA8 },
            sprtext = { 0xFE, 0x52, 0x5D, 0x52, 0x5E },
            sprtext2 = { 0xF7, 0xA1, 0x23, 0xA0, 0xA4 },
            sprno1 = { 32, 96, 32, 32, 32, 32, 112 },
            sprno2 = { 112, 136, 8, 16, 32, 64, 248 },
            sprno3 = { 112, 136, 8, 48, 8, 136, 112 },
            sprblank = { 0, 0, 0, 0, 0, 0, 0, 0 },
        #endregion
        #region playfields
            pfbase = { 255, 255, 240,
                        128, 0, 16,
                        128, 0, 16,
                        128, 0, 16,
                        128, 0, 16,
                        128, 0, 16,
                        255, 255, 240 },
            pfenterup = { 0xFF, 0x0F, 0xF0,
                            0x80, 0x00, 0x10,
                            0x80, 0x00, 0x10,
                            0x80, 0x00, 0x10,
                            0x80, 0x00, 0x10,
                            0x80, 0x00, 0x10,
                            0xFF, 0xFF, 0xF0 },
            pfenterdown = { 0xFF, 0xFF, 0xF0,
                            0x80, 0x00, 0x10,
                            0x80, 0x00, 0x10,
                            0x80, 0x00, 0x10,
                            0x80, 0x00, 0x10,
                            0x80, 0x00, 0x10,
                            0xFF, 0x0F, 0xF0 },
            pfhallway = { 0xFF, 0x0F, 0xF0,
                            0, 0, 0, 0, 0, 0,
                            0, 0, 0, 0, 0, 0,
                            0, 0, 0,
                            0xFF, 0x0F, 0xF0 }
        #endregion region
            ;
    }
}
