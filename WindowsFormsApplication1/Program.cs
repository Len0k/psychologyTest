
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Collections.Generic;

class Character {
    public int id;
    public PictureBox pb;
    public int coast;
    public bool inDaBoat;
    public int pos;
    public Character() {
        id = coast = 0;
        inDaBoat = false;
        pb = new PictureBox();
        pb.Size = new Size(56, 105);
        pb.TabStop = false;
        pb.Tag = id;
    }

    public Character(int s, int p, Point c, int tag) {
        id = s;
        coast = 0;
        inDaBoat = false;
        pb = new PictureBox();
        pb.Location = c;
        pb.Size = new Size(56, 105);
        pb.BackColor = Color.Transparent;
        pb.Image = (System.Drawing.Image)RiverTask.resourses.GetObject("_"+id.ToString());
        pb.Visible = true;
        pb.TabStop = false;
        pb.Tag = tag;
        pos = p;
    }
}

public class RiverTask : System.Windows.Forms.Form {
    private Character[] chars;
    private Boat boat;
    //private System.Windows.Forms.Label promptLabel;

    public RiverTask() {
        InitializeComponent();
    }
    private Point coord(int i) { // Magic constants - coords on coasts
        switch (i) {
            case 0: return new Point(5, 150);
            case 1: return new Point(75, 160);
            case 2: return new Point(138, 140);
            case 3: return new Point(210, 158);
            case 4: return new Point(137, 241);
            case 5: return new Point(15, 240);
            case 6: return new Point(270, 180);
            case 7: return new Point(84, 269);
            case 8: return new Point(575, 137);
            case 9: return new Point(653, 134);
            case 10: return new Point(724, 126);
            case 11: return new Point(630, 252);
            case 12: return new Point(690, 252);
            case 13: return new Point(750, 255);
            case 14: return new Point(620, 360);
            default: return new Point(721, 363);
        }
    }
    private bool[] placeIsFree;
    private int getPlace(int coast = 0) {
        for (int i = (coast << 3); i < (coast << 3) + 8; ++i) {
            if (placeIsFree[i]) {
                placeIsFree[i] = false;
                return i;
            }
        }
        return -1;
    }

    public static System.ComponentModel.ComponentResourceManager resourses = new ComponentResourceManager(typeof(WindowsFormsApplication1.Properties.Resources));

    private void InitializeComponent() {
        this.Size = new Size(807, 500);
        ShowLabel("Hello!");
        //resourses = new ComponentResourceManager(typeof(WindowsFormsApplication1.Properties.Resources));
        this.chars = new Character[8];
        boat = new Boat();
        boat.pb.BackColor = Color.Transparent;
        boat.pb.Image = (System.Drawing.Image)resourses.GetObject("plot");
        boat.pb.Click += new EventHandler(boat_Click);
        Controls.Add(boat.pb);
        placeIsFree = new bool[] { true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true };
        this.BackgroundImage = Image.FromFile(Directory.GetCurrentDirectory() + "\\psyho.png");
        int chId = 1;
        for (int i = 0; i < 8; ++i) {
            int pos = getPlace();
            this.chars[i] = new Character(chId, pos, coord(pos), i);
            this.chars[i].pb.Click += new EventHandler(pb_Click);
            this.Controls.Add(this.chars[i].pb);
            chId = chId << 1;
        }

        this.SuspendLayout();
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
        this.Text = "PsycholigyTest";
        this.ResumeLayout(false);
    }
    [STAThread]
    static void Main() {
        Application.Run(new RiverTask());
    }
    public int getIndex(int id) {
        switch (id) {
            case 1: return 0;
            case 2: return 1;
            case 4: return 2;
            case 8: return 3;
            case 16: return 4;
            case 32: return 5;
            case 64: return 6;
            default: return 7;
        }
    }

    private void pb_Click(object sender, EventArgs e) {
        Character ch = chars[(int)((PictureBox)sender).Tag];
        int id = ch.id;
        if (ch.inDaBoat) {
            boat.unput(id);
            ch.inDaBoat = false;
            ch.coast = boat.coast;
            ch.pos = getPlace(ch.coast);
            ch.pb.Location = coord(ch.pos);
        } else {
            Point p = boat.put(id, ch.coast);
            if (p.X != 0 && p.Y != 0) {
                ch.pb.Location = p;
                ch.inDaBoat = true;
                placeIsFree[ch.pos] = true;
            }
        }
    }
    private static int erCounter = 0;
    private static int exCounter = 0;
    private void ShowWrongMove() {
        ++erCounter;
        ShowLabel("Этот ход не правильный!");
    }
    private void ShowExtraMove() {
        ++exCounter;
        ShowLabel("Этот ход допустим по правилам, но является лишним в алгоритме. Придумайте другой!");
    }
    private void ShowLabel(string msg) {
        Label L = new Label();
        L.BackColor = Color.Transparent;
        L.Location = new Point(0, 0);
        L.Size = this.Size;
        L.Text = msg;
        L.Font = new Font(System.Drawing.FontFamily.GenericSansSerif, 20);
        L.TextAlign = ContentAlignment.TopCenter;
        L.ForeColor = Color.DarkBlue;
        L.Click += new EventHandler(L_Click);
        this.Controls.Add(L);
        L.BringToFront();
    }

    void L_Click(object sender, EventArgs e) {
        Label L = (Label)sender;
        Controls.Remove(L);
        L.Dispose();
    }

    private void boat_Click(object sender, EventArgs e) {
        //if (boat.count == 0) return;//(CheckMove.CheckMoving(boat) != 0) return;
        int t = CheckMove.CheckMoving(boat);
        if (t == 2) {
            ShowWrongMove();
            return;
        }
        if (t == 1) {
            ShowExtraMove();
            return;
        }
        if (t == 0) {
            boat.count = 0;
            ++boat.coast;
            boat.coast &= 1;
            Character ch1 = chars[getIndex(boat.id[0])];
            ch1.inDaBoat = false;
            ch1.coast = boat.coast;
            ch1.pos = getPlace(ch1.coast);
            ch1.pb.Location = coord(ch1.pos);
            boat.id[0] = 0;
            boat.pb.Location = boat.loca[boat.coast];
            boat.coord[0] = new Point(boat.pb.Location.X + 20, boat.pb.Location.Y - 83);
            boat.coord[1] = new Point(boat.pb.Location.X + 75, boat.pb.Location.Y - 83);
            if (boat.id[1] == 0) return;
            Character ch2 = chars[getIndex(boat.id[1])];
            ch2.inDaBoat = false;
            ch2.coast = boat.coast;
            ch2.pos = getPlace(ch2.coast);
            ch2.pb.Location = coord(ch2.pos);
            boat.id[1] = 0;
        }
    }

}

class Boat {
    public Boat() {
        coast = count = 0;
        id = new int[] { 0, 0 };
        pb = new PictureBox();
        loca = new Point[2];
        loca[0] = new Point(260, 313);
        loca[1] = new Point(490, 309);
        pb.Location = loca[0];
        coord = new Point[2];
        coord[0] = new Point(280, 230);
        coord[1] = new Point(335, 230);
        pb.Size = new Size(132, 41);
    }
    public int coast;
    public int count;
    public int[] id;
    public Point[] coord;
    public Point[] loca;
    public PictureBox pb;
    public Point put(int chId, int chCoast) {
        if (count < 2 && chCoast == coast) {
            id[count] = chId;
            ++count;
            return coord[count - 1];
        } else return new Point(0, 0);
    }
    public void unput(int chId) {
        if (id[0] == chId) {
            Point temp = new Point(coord[0].X, coord[0].Y);
            coord[0] = new Point(coord[1].X, coord[1].Y);
            coord[1] = temp;
            --count;
            id[0] = id[1];
            id[1] = 0;
        } else {
            --count;
            id[1] = 0;
        }
    }
}


class CheckMove {
    //7,4,1 - папа, мама, полицейский
    //6,5,3,2,0 - сын1,сын2,дочь1,дочь2,хулиган (или как его там)
    //(это номер бита, который соответствует персонажу)
    //Последовательность ходов
    //000 000 00   0    +3
    //000 000 11   3    -2
    //000 000 01   1    +66 (+34)   +10 (+6)
    //010 000 11   67   -3
    //010 000 00   64   +160 (+192) +20 (+24)
    //111 000 00   224  -128        -16
    //011 000 00   96   +144
    //111 100 00   240  -16         -128
    //111 000 00   224  +3
    //111 000 11   227  -128        -16
    //011 000 11   99   +144
    //111 100 11   243  -16         -128
    //111 000 11   227  +24 (+20)   +192 (+160)
    //111 110 11   251  -3
    //111 110 00   248  +6 (+10)    +34 (+66)
    //111 111 10   254  -2
    //111 111 00   252  +3
    //111 111 11   255
    //public List<int> OldPeople;// = new List<int> { 1 << 7, 1 << 4, 1 << 1 };
    public static int[,] MoveSequence = //new int[,] 
    {
        { 3, -2, 66, -3, 160, -128, 144, -16, 3, -128, 144, -16, 24, -3, 6, -2, 3 },
        { 3, -2, 66, -3, 160, -128, 144, -16, 3, -128, 144, -16, 20, -3, 10, -2, 3 },
        { 3, -2, 34, -3, 192, -128, 144, -16, 3, -128, 144, -16, 24, -3, 6, -2, 3 },
        { 3, -2, 34, -3, 192, -128, 144, -16, 3, -128, 144, -16, 20, -3, 10, -2, 3 },
        { 3, -2, 10, -3, 20, -16, 144, -128, 3, -16, 144, -128, 192, -3, 34, -2, 3 },
        { 3, -2, 10, -3, 20, -16, 144, -128, 3, -16, 144, -128, 160, -3, 66, -2, 3 },
        { 3, -2, 6, -3, 24, -16, 144, -128, 3, -16, 144, -128, 192, -3, 34, -2, 3 },
        { 3, -2, 6, -3, 24, -16, 144, -128, 3, -16, 144, -128, 160, -3, 66, -2, 3 },
    };
    public static int CurrentStep = 0;
    public static int CurrentState = 0;
    public static int CurrentWay = 0;
    static bool IsBoatCorrect(Boat boat) {
        //List<int> YoungPeople = new List<int> { 1 << 6, 1 << 5, 1 << 3, 1 << 2, 1 << 0, 0};
        List<int> OldPeople = new List<int> { 1 << 7, 1 << 4, 1 << 1 };
        //if (YoungPeople.Contains(boat.id[0]) && (boat.count == 1 ? true : (YoungPeople.Contains(boat.id[1]))))
        //if (YoungPeople.Contains(boat.id[0]) && YoungPeople.Contains(boat.id[1]))
        //    return false;//посадите в лодку хотя бы одного взрослого
        //return true;
        return (OldPeople.Contains(boat.id[0]) || OldPeople.Contains(boat.id[1]));
    }
    public static int CheckMoving(Boat vodka)//я не помню че сюда передается, но надеюсь, что Ид персонажей, которые я надеюсь совпадают с теми которые написаны выше
    {
        if (!IsBoatCorrect(vodka))
            return 2;//аасссссипка
        int trololo = vodka.id[0] + vodka.id[1];//(1 << vodka.id[0]) + (vodka.count == 2 ? (1 << vodka.id[1]) : 0);
        //окай
        //if (vodka.coast == 1)
        trololo = (1 - vodka.coast * 2) * trololo;//надеюсь 0 - это начальный берег, а 1 - яннп
        int tmpVariable = CurrentState + trololo;
        String о_О = Convert.ToString(tmpVariable, 2);
        о_О = о_О.PadLeft(8, '0');
        if ((о_О[6] != о_О[7] && tmpVariable != 1 && tmpVariable != 254) || (о_О[3] != о_О[0] && ((о_О[0] != о_О[1]) || (о_О[0] != о_О[2]) || (о_О[3] != о_О[4]) || (о_О[3] != о_О[5]))))
            return 2;//некоректный ход
        int tmpCW = CurrentWay;
        while (tmpCW < 8)
        {
            if (trololo == MoveSequence[tmpCW, CurrentStep])
            {
                CurrentWay = tmpCW;
                break;
            }
            tmpCW++;
        }
        if (trololo == MoveSequence[CurrentWay, CurrentStep])
        {
            CurrentState += trololo;
            CurrentStep++;
            return 0;//правильный ход
        }
        return 1;//лишний ход
    }
}
