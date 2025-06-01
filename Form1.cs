using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace FinalOdevi
{
    public partial class RobotSimülasyonu : Form
    {
        public RobotSimülasyonu()
        {
            InitializeComponent();
            //robotu oluştururken yönünü belirlemek için comboBox elemanları:
            comboBox1.Items.Add("K");
            comboBox1.Items.Add("G");
            comboBox1.Items.Add("D");
            comboBox1.Items.Add("B");
            pictureBox1.Size = new Size(420, 420);
        }

        private void Çıkış_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(pictureBox1.Size.ToString(), "BİLGİLENDİRME", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        private void RobotSimülasyonu_Load(object sender, EventArgs e)
        {
            groupBox3.Enabled = false;
            groupBox4.Enabled = false;
        }

        /// <summary>
        /// PictureBox taki koordinat düzlemini oluşturmaya yarayan fonksiyon ve zemin için boyut bilgisini parametre olarak alır.
        /// </summary>
        public void ZeminOlustur(int boyut)
        {
            Graphics g = pictureBox1.CreateGraphics();
            g.Clear(Color.White);
            Pen kalem = new Pen(Color.Black, 2);
            Brush fırca = new SolidBrush(Color.Black);
            int aralıkDegeri = (pictureBox1.Size.Width - 20) / boyut; // 400/5=80

            // 420,420 size a sahip picture box ı bolme ve sayılarını ekleme
            for (int i = 0, x = boyut - 1; i < boyut; i++, x--)
            {
                g.DrawLine(kalem, new Point(20, i * aralıkDegeri), new Point(420, i * aralıkDegeri)); // yatay çizgiler 
                g.DrawString(x.ToString(), new Font("Arial", 12), fırca, new Point(5, (aralıkDegeri / 2) + (i * aralıkDegeri)));//sayılar
            }
            for (int i = 0; i < boyut; i++)
            {
                g.DrawLine(kalem, new Point(20 + (i * aralıkDegeri), 0), new Point(20 + (i * aralıkDegeri), 400));//dikey çizgiler
                g.DrawString(i.ToString(), new Font("Arial", 12), fırca, new Point(20 + (aralıkDegeri / 2) + (i * aralıkDegeri), 405)); //sayılar
            }
        }


        /// <summary>
        /// Robotu ilk olarak oluşturmak ve konumlandırmak için kullanılan fonksiyondur. boyut bilgisini parametre olarak alır. 
        /// </summary>
        Point RobotKonumNow;
        string RobotYönNow;
        public void RobotuKonumlandır(int boyut)
        {
            Graphics g = pictureBox1.CreateGraphics();
            Brush fırca = new SolidBrush(Color.DarkBlue);

            if (int.TryParse(RbtKnmX.Text, out int x) && int.TryParse(RbtKnmY.Text, out int y))
            {
                if (x < 0 || y < 0 || x > boyut - 1 || y > boyut - 1)
                {
                    MessageBox.Show("Lütfen geçerli bir konum giriniz.");
                    return;
                }

                int aralıkDegeri = (pictureBox1.Size.Width - 20) / boyut;
                int xKonum = 20 + (x * aralıkDegeri);
                int yKonum = ((boyut - 1 - y) * aralıkDegeri); // y ekseni ters olduğu için bu şekilde hesaplanıyor
                RobotKonumNow = new Point(xKonum, yKonum);
                RobotYönNow = comboBox1.SelectedItem.ToString();
                Rectangle robot = new Rectangle(xKonum, yKonum, aralıkDegeri, aralıkDegeri);
                g.FillEllipse(fırca, robot);

                Point merkez = new Point(xKonum + (aralıkDegeri / 2), yKonum + (aralıkDegeri / 2));
                Pen kalem2 = new Pen(Color.Red, 2);
                if (comboBox1.SelectedItem == "K")
                {
                    Point point = new Point(merkez.X, merkez.Y - aralıkDegeri / 2); // yukarı doğru yönü
                    g.DrawLine(kalem2, merkez, point);
                    RobotYönNow = "K"; // Yönü Kuzey olarak güncelle
                }
                else if (comboBox1.SelectedItem == "G")
                {
                    Point point = new Point(merkez.X, merkez.Y + aralıkDegeri / 2); // aşağı doğru yönü
                    g.DrawLine(kalem2, merkez, point);
                    RobotYönNow = "G"; // Yönü Güney olarak güncelle
                }
                else if (comboBox1.SelectedItem == "D")
                {
                    Point point = new Point(merkez.X + aralıkDegeri / 2, merkez.Y); // sağa doğru yönü
                    g.DrawLine(kalem2, merkez, point);
                    RobotYönNow = "D"; // Yönü Doğu olarak güncelle
                }
                else if (comboBox1.SelectedItem == "B")
                {
                    Point point = new Point(merkez.X - aralıkDegeri / 2, merkez.Y); // sola doğru yönü
                    g.DrawLine(kalem2, merkez, point);
                    RobotYönNow = "B";
                }
                else
                {
                    MessageBox.Show("Lütfen bir yön seçiniz.", "HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Lütfen X ve Y koordinatlarını sayısal olarak giriniz.");
                return;
            }
        }

       
        /// <summary>
        /// Ulaşılması gereken hedef daireyi konumlandıran bir fonksiyondur. boyutu parametre olarak alır.
        /// </summary>
        Point HedefinKonumNow;
        public void HedefKonumlandır(int boyut)
        {
            Graphics g = pictureBox1.CreateGraphics();
            Pen kalem = new Pen(Color.Green, 2);
            Brush fırca = new SolidBrush(Color.Green);

            if (int.TryParse(HdfKnmX.Text, out int x) && int.TryParse(HdfKnmY.Text, out int y))
            {
                if (x < 0 || y < 0 || x > boyut - 1 || y > boyut - 1)
                {
                    MessageBox.Show("Lütfen geçerli bir konum giriniz.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else if (x == Convert.ToInt32(RbtKnmX.Text) && y == Convert.ToInt32(RbtKnmY.Text))
                {
                    MessageBox.Show("Robotun bulunduğu konuma hedef ekleyemezsiniz.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
               
                int aralıkDegeri = (pictureBox1.Size.Width - 20) / boyut;
                int xKonum = 20 + (x * aralıkDegeri);
                int yKonum = ((boyut - 1 - y) * aralıkDegeri); // y ekseni ters olduğu için bu şekilde hesaplanıyor
                HedefinKonumNow = new Point(xKonum, yKonum);
                Rectangle hedef = new Rectangle(xKonum, yKonum, aralıkDegeri, aralıkDegeri);
                g.FillEllipse(fırca, hedef);
            }
            else
            {
                MessageBox.Show("Lütfen X ve Y koordinatlarını sayısal olarak giriniz.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            HdfKnmX.Enabled = false;
            HdfKnmY.Enabled = false;
        }


        /// <summary>
        /// Konumlandırma butonu click event
        /// </summary>
        private void Konumlandır_Click(object sender, EventArgs e)
        {
            groupBox3.Enabled = true;
            groupBox4.Enabled = true;

            pictureBox1.Refresh();
            ListViewKolonOlustur();
            int boyut;
            if (int.TryParse(HrtBytTextBox.Text, out boyut))
            {
                ZeminOlustur(boyut);
                RobotuKonumlandır(boyut);
                HedefKonumlandır(boyut);
            }
            else
            {
                MessageBox.Show("Lütfen geçerli bir boyut giriniz.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            ListViewElemanEkle("Başlangıç");
            Konumlandır.Enabled = false;
        }


        /// <summary>
        /// Robotun önündeki engelleri konumlandıran fonksiyondur. boyut bilgisini parametre olarak alır.
        /// </summary>
        int engelSayisi = 0;
        Point EngelKonumNow;
        public void EngelOlustur(int boyut)
        {
            if (engelSayisi >= boyut)
            {
                MessageBox.Show("En fazla " + boyut + "  engel ekleyebilirsiniz.", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                EngelEkle.Enabled = false;
                return;
            }
            Graphics g = pictureBox1.CreateGraphics();
            Pen kalem = new Pen(Color.Black, 2);
            Brush fırca = new SolidBrush(Color.Black);

            if (int.TryParse(EnglKnmX.Text, out int x) && int.TryParse(EnglKnmY.Text, out int y))
            {
                if (x < 0 || y < 0 || x > boyut - 1 || y > boyut - 1)
                {
                    MessageBox.Show("Lütfen geçerli bir konum giriniz.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else if (x==Convert.ToInt32(RbtKnmX.Text) && y==Convert.ToInt32(RbtKnmY.Text))
                {
                    MessageBox.Show("Robotun bulunduğu konuma engel ekleyemezsiniz.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else if(x == Convert.ToInt32(HdfKnmX.Text) && y == Convert.ToInt32(HdfKnmY.Text))
                {
                    MessageBox.Show("Hedefin bulunduğu konuma engel ekleyemezsiniz.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int aralıkDegeri = (pictureBox1.Size.Width - 20) / boyut;
                int xKonum = 20 + (x * aralıkDegeri);
                int yKonum = ((boyut - 1 - y) * aralıkDegeri); // y ekseni ters olduğu için bu şekilde hesaplanıyor
                EngelKonumNow = new Point(xKonum, yKonum);
                Rectangle engel = new Rectangle(xKonum, yKonum, aralıkDegeri, aralıkDegeri);
                g.FillRectangle(fırca, engel);
                engelSayisi++;
            }
            else
            {
                MessageBox.Show("Lütfen X ve Y koordinatlarını sayısal olarak giriniz.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }



        /// <summary>
        /// Engel eklemek için kullanılan butonun click eventı
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EngelEkle_Click(object sender, EventArgs e)
        {
            int boyut;
            if (int.TryParse(HrtBytTextBox.Text, out boyut))
            {
                EngelOlustur(boyut);
            }
            else
            {
                MessageBox.Show("Lütfen geçerli bir boyut giriniz.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        int listelemeDurumu = 1;
        /// <summary>
        /// Robotu hareket ettirmek için girilen komut U ise yukarı 1 br hareket etmesini sağlayan fonksiyondur.
        /// </summary>
        /// <param name="yön"></param>
        /// <param name="boyut"></param>
        public void komutUP(string yön, int boyut) //Yukarı hareket ettirmek için
        {
            int aralıkDegeri = (pictureBox1.Size.Width - 20) / boyut;
            Graphics g = pictureBox1.CreateGraphics();
            int robotX = RobotKonumNow.X;
            int robotY = RobotKonumNow.Y;
            int engelX = EngelKonumNow.X;
            int engelY = EngelKonumNow.Y;
            int hedefX = HedefinKonumNow.X;
            int hedefY = HedefinKonumNow.Y;

            robotY -= aralıkDegeri; // yukarı hareket ettirmek için Y koordinatını azaltıyoruz

            Rectangle RobotKonumYeni = new Rectangle(robotX, robotY, aralıkDegeri, aralıkDegeri); //robotun hareket edince oluşacak konum bilgisi
            Point merkez = new Point(RobotKonumNow.X + (aralıkDegeri / 2), RobotKonumNow.Y + (aralıkDegeri / 2));

            if (RobotKonumYeni.X == engelX && RobotKonumYeni.Y == engelY)
            {
                MessageBox.Show("Robot engel ile karşılaştı! Lütfen farklı bir komut deneyin.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                listelemeDurumu = 0;
                return;
            }
            else
            {
                // Eski konumu temizle
                g.FillEllipse(Brushes.White, new Rectangle(RobotKonumNow.X, RobotKonumNow.Y, aralıkDegeri , aralıkDegeri ));
                g.FillEllipse(new SolidBrush(Color.DarkBlue), RobotKonumYeni);

                listelemeDurumu = 1;
                Pen kalem = new Pen(Color.Red, 2);
                RobotKonumNow.X = RobotKonumYeni.X;
                RobotKonumNow.Y = RobotKonumYeni.Y;
                merkez = new Point(RobotKonumNow.X + (aralıkDegeri / 2), RobotKonumNow.Y + (aralıkDegeri / 2)); //merkezi güncelle
                if (yön == "K") //Kuzey
                {
                    Point point = new Point(merkez.X, merkez.Y - aralıkDegeri / 2); // yukarı doğru yönü
                    g.DrawLine(kalem, merkez, point);
                    RobotYönNow = "K"; // Yönü Kuzey olarak güncelle
                }
                else if (yön == "G") //Güney
                {
                    Point point = new Point(merkez.X, merkez.Y + aralıkDegeri / 2); // aşağı doğru yönü
                    g.DrawLine(kalem, merkez, point);
                    RobotYönNow = "G"; // Yönü Güney olarak güncelle
                }
                else if (yön == "D") //Doğu
                {
                    Point point = new Point(merkez.X + (aralıkDegeri / 2), merkez.Y); // sağa doğru yönü
                    g.DrawLine(kalem, merkez, point);
                    RobotYönNow = "D"; // Yönü Doğu olarak güncelle
                }
                else if (yön == "B") //Batı
                {
                    Point point = new Point(merkez.X - (aralıkDegeri / 2), merkez.Y); // sola doğru yönü
                    g.DrawLine(kalem, merkez, point);
                    RobotYönNow = "B"; // Yönü Batı olarak güncelle
                }

                if (RobotKonumYeni.X == hedefX && RobotKonumYeni.Y == hedefY)
                {
                    MessageBox.Show("TEBRİKLER! Robot hedefe ulaştı!", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }   
        }
         

        /// <summary>
        /// Robotu hareket ettirmek için girilen komut D ise aşağı 1 br hareket etmesini sağlayan fonksiyondur.
        /// </summary>
        /// <param name="yön"></param>
        /// <param name="boyut"></param>
        public void komutDOWN(string yön, int boyut) //Aşağı hareket ettirmek için
        {
            int aralıkDegeri = (pictureBox1.Size.Width - 20) / boyut;
            Graphics g = pictureBox1.CreateGraphics();
            int robotX = RobotKonumNow.X;
            int robotY = RobotKonumNow.Y;
            int engelX = EngelKonumNow.X;
            int engelY = EngelKonumNow.Y;
            int hedefX = HedefinKonumNow.X;
            int hedefY = HedefinKonumNow.Y;

            robotY += aralıkDegeri; // aşağı hareket ettirmek için Y koordinatını artırıyoruz
            Point merkez = new Point(RobotKonumNow.X + (aralıkDegeri / 2), RobotKonumNow.Y + (aralıkDegeri / 2));
            Rectangle RobotKonumYeni = new Rectangle(robotX, robotY, aralıkDegeri, aralıkDegeri); //robotun hareket edince oluşacak konum bilgisi

            if (RobotKonumYeni.X == engelX && RobotKonumYeni.Y == engelY)
            {
                MessageBox.Show("Robot engel ile karşılaştı! Lütfen farklı bir komut deneyin.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                listelemeDurumu = 0;
                return;
            }
            else 
            {
                // Eski konumu temizle
                g.FillEllipse(Brushes.White, new Rectangle(RobotKonumNow.X, RobotKonumNow.Y, aralıkDegeri, aralıkDegeri));
                g.FillEllipse(new SolidBrush(Color.DarkBlue), RobotKonumYeni);

                listelemeDurumu = 1;
                RobotKonumNow.X = RobotKonumYeni.X;
                RobotKonumNow.Y = RobotKonumYeni.Y;
                merkez = new Point(RobotKonumNow.X + (aralıkDegeri / 2), RobotKonumNow.Y + (aralıkDegeri / 2)); //merkezi güncelle

                Pen Kalem = new Pen(Color.Red, 2);

                if (yön == "K") // Kuzey (yukarı)
                {
                    Point hedef = new Point(merkez.X, merkez.Y - (aralıkDegeri / 2));
                    g.DrawLine(Kalem, merkez, hedef);
                    RobotYönNow = "K"; 
                }
                else if (yön == "G") // Güney (aşağı)
                {
                    Point hedef = new Point(merkez.X, merkez.Y + (aralıkDegeri / 2));
                    g.DrawLine(Kalem, merkez, hedef);
                    RobotYönNow = "G"; // Yönü Güney olarak güncelle
                }
                else if (yön == "D") // Doğu (sağ)
                {
                    Point hedef = new Point(merkez.X + (aralıkDegeri / 2), merkez.Y);
                    g.DrawLine(Kalem, merkez, hedef);
                    RobotYönNow = "D"; // Yönü Doğu olarak güncelle
                }
                else if (yön == "B") // Batı (sol)
                {
                    Point hedef = new Point(merkez.X - (aralıkDegeri / 2), merkez.Y);
                    g.DrawLine(Kalem, merkez, hedef);
                    RobotYönNow = "B"; // Yönü Batı olarak güncelle
                }

                if (RobotKonumYeni.X == hedefX && RobotKonumYeni.Y == hedefY)
                {
                    MessageBox.Show("TEBRİKLER! Robot hedefe ulaştı!", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
        }


        /// <summary>
        /// Robotu hareket ettirmek için girilen komut L ise sola 1 br hareket etmesini sağlayan fonksiyondur.
        /// </summary>
        /// <param name="yön"></param>
        /// <param name="boyut"></param>
        public void komutLEFT(string yön, int boyut) //SOLA hareket ettirmek için
        {
            int aralıkDegeri = (pictureBox1.Size.Width - 20) / boyut;
            Graphics g = pictureBox1.CreateGraphics();
            int robotX = RobotKonumNow.X;
            int robotY = RobotKonumNow.Y;
            int engelX = EngelKonumNow.X;
            int engelY = EngelKonumNow.Y;
            int hedefX = HedefinKonumNow.X;
            int hedefY = HedefinKonumNow.Y;

            robotX -= aralıkDegeri; // sola hareket ettirmek için X koordinatını azaltıyoruz

            Rectangle RobotKonumYeni = new Rectangle(robotX, robotY, aralıkDegeri, aralıkDegeri); //robotun hareket edince oluşacak konum bilgisi
            Point merkez = new Point(RobotKonumNow.X + (aralıkDegeri / 2), RobotKonumNow.Y + (aralıkDegeri / 2));

            if (RobotKonumYeni.X == engelX && RobotKonumYeni.Y == engelY)
            {
                MessageBox.Show("Robot engel ile karşılaştı! Lütfen farklı bir komut deneyin.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                listelemeDurumu = 0;
                return;
            }
            else 
            {
                // Eski konumu temizle
                g.FillEllipse(Brushes.White, new Rectangle(RobotKonumNow.X, RobotKonumNow.Y, aralıkDegeri, aralıkDegeri));
                g.FillEllipse(new SolidBrush(Color.DarkBlue), RobotKonumYeni);

                listelemeDurumu = 1;
                RobotKonumNow.X = RobotKonumYeni.X;
                RobotKonumNow.Y = RobotKonumYeni.Y;
                merkez = new Point(RobotKonumNow.X + (aralıkDegeri / 2), RobotKonumNow.Y + (aralıkDegeri / 2)); //merkezi güncelle

                Pen Kalem = new Pen(Color.Red, 2);
                if (yön == "K") // Kuzey (yukarı)
                {
                    Point hedef = new Point(merkez.X, merkez.Y - (aralıkDegeri / 2));
                    g.DrawLine(Kalem, merkez, hedef);
                    RobotYönNow = "K"; // Yönü Kuzey olarak güncelle
                }
                else if (yön == "G") // Güney (aşağı)
                {
                    Point hedef = new Point(merkez.X, merkez.Y + (aralıkDegeri / 2));
                    g.DrawLine(Kalem, merkez, hedef);
                    RobotYönNow = "G"; // Yönü Güney olarak güncelle
                }
                else if (yön == "D") // Doğu (sağ)
                {
                    Point hedef = new Point(merkez.X + (aralıkDegeri / 2), merkez.Y);
                    g.DrawLine(Kalem, merkez, hedef);
                    RobotYönNow = "D"; // Yönü Doğu olarak güncelle
                }
                else if (yön == "B") // Batı (sol)
                {
                    Point hedef = new Point(merkez.X - (aralıkDegeri / 2), merkez.Y);
                    g.DrawLine(Kalem, merkez, hedef);
                    RobotYönNow = "B"; // Yönü Batı olarak güncelle
                }
                
                if (RobotKonumYeni.X == hedefX && RobotKonumYeni.Y == hedefY)
                {
                    MessageBox.Show("TEBRİKLER! Robot hedefe ulaştı!", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
        }


        /// <summary>
        /// Robotu hareket ettirmek için girilen komut L ise sola 1 br hareket etmesini sağlayan fonksiyondur.
        /// </summary>
        /// <param name="yön"></param>
        /// <param name="boyut"></param>
        public void komutRIGHT(string yön, int boyut) //SAĞA hareket ettirmek için
        {
            int aralıkDegeri = (pictureBox1.Size.Width - 20) / boyut;
            Graphics g = pictureBox1.CreateGraphics();
            int robotX = RobotKonumNow.X;
            int robotY = RobotKonumNow.Y;
            int engelX = EngelKonumNow.X;
            int engelY = EngelKonumNow.Y;
            int hedefX = HedefinKonumNow.X;
            int hedefY = HedefinKonumNow.Y;

            robotX += aralıkDegeri; // sola hareket ettirmek için X koordinatını azaltıyoruz

            Rectangle RobotKonumYeni = new Rectangle(robotX, robotY, aralıkDegeri, aralıkDegeri); //robotun hareket edince oluşacak konum bilgisi
            Point merkez = new Point(RobotKonumNow.X + (aralıkDegeri / 2), RobotKonumNow.Y + (aralıkDegeri / 2));

            if (RobotKonumYeni.X == engelX && RobotKonumYeni.Y == engelY)
            {
                MessageBox.Show("Robot engel ile karşılaştı! Lütfen farklı bir komut deneyin.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                listelemeDurumu = 0;
                return;
            }
            else 
            {
                // Eski konumu temizle
                g.FillEllipse(Brushes.White, new Rectangle(RobotKonumNow.X, RobotKonumNow.Y, aralıkDegeri, aralıkDegeri));
                g.FillEllipse(new SolidBrush(Color.DarkBlue), RobotKonumYeni);

                listelemeDurumu = 1;
                RobotKonumNow.X = RobotKonumYeni.X;
                RobotKonumNow.Y = RobotKonumYeni.Y;
                merkez = new Point(RobotKonumNow.X + (aralıkDegeri / 2), RobotKonumNow.Y + (aralıkDegeri / 2)); //merkezi güncelle

                Pen Kalem = new Pen(Color.Red, 2);
                if (yön == "K") // Kuzey (yukarı)
                {
                    Point hedef = new Point(merkez.X, merkez.Y - (aralıkDegeri / 2));
                    g.DrawLine(Kalem, merkez, hedef);
                    RobotYönNow = "K"; // Yönü Kuzey olarak güncelle
                }
                else if (yön == "G") // Güney (aşağı)
                {
                    Point hedef = new Point(merkez.X, merkez.Y + (aralıkDegeri / 2));
                    g.DrawLine(Kalem, merkez, hedef);
                    RobotYönNow = "G"; // Yönü Güney olarak güncelle
                }
                else if (yön == "D") // Doğu (sağ)
                {
                    Point hedef = new Point(merkez.X + (aralıkDegeri / 2), merkez.Y);
                    g.DrawLine(Kalem, merkez, hedef);
                    RobotYönNow = "D"; // Yönü Doğu olarak güncelle
                }
                else if (yön == "B") // Batı (sol)
                {
                    Point hedef = new Point(merkez.X - (aralıkDegeri / 2), merkez.Y);
                    g.DrawLine(Kalem, merkez, hedef);
                    RobotYönNow = "B"; // Yönü Batı olarak güncelle
                }
                
                if (RobotKonumYeni.X == hedefX && RobotKonumYeni.Y == hedefY)
                {
                    MessageBox.Show("TEBRİKLER! Robot hedefe ulaştı!", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
        }



        /// <summary>
        /// Robotun saat yönünde 90 derdön 1br ileri gitmesini sağlayan fonksiyndur. Robotun hareket temeden önceki yönünü parametre olarak alır
        /// </summary>
        /// <param name="yön"></param>
        /// <param name="boyut"></param>
        public void komutTURN(string yön, int boyut) //saat yönünde 90 derdön 1br ileri git
        {
            Graphics g = pictureBox1.CreateGraphics();

            if (yön == "K")
            {
                yön = "D";
                komutRIGHT(yön, boyut); //ileri hareket ettir.
            }
            else if (yön == "D")
            {
                yön = "G";
                komutDOWN(yön, boyut); //ileri hareket ettir.
            }
            else if (yön == "G")
            {
                yön = "B";
                komutLEFT(yön, boyut); //ileri hareket ettir.
            }
            else if (yön == "B")
            {
                yön = "K";
                komutUP(yön, boyut); //ileri hareket ettir.
            }
            else
            {
                MessageBox.Show("Lütfen geçerli bir yön giriniz.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }


        /// <summary>
        /// Robotun saat yönünün tersine 90 derdön 1br ileri gitmesini sağlayan fonksiyondur. Robotun hareket etmeden önceki yönünü parametre olarak alır
        /// </summary>
        /// <param name="yön"></param>
        /// <param name="boyut"></param>
        public void komutRETURN(string yön, int boyut) //saat yönünün tersine 90 der dön 1 br ilei git
        {
            Graphics g = pictureBox1.CreateGraphics();

            if (yön == "K")
            {
                yön = "B";
                komutLEFT(yön, boyut); //ileri hareket ettir.
            }
            else if (yön == "D")
            {
                yön = "K";
                komutUP(yön, boyut); //ileri hareket ettir.
            }
            else if (yön == "G")
            {
                yön = "D";
                komutRIGHT(yön, boyut); //ileri hareket ettir.
            }
            else if (yön == "B")
            {
                yön = "G";
                komutDOWN(yön, boyut); //ileri hareket ettir.
            }
            else
            {
                MessageBox.Show("Lütfen geçerli bir yön giriniz.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }


        /// <summary>
        /// ListView Kolon oluştuma ve eleman ekleme fonksiyonları
        /// </summary>
        public void ListViewKolonOlustur()
        {
            listView1.View = View.Details;

            listView1.Columns.Add("Adım No", 60);
            listView1.Columns.Add("X", 60);
            listView1.Columns.Add("Y", 60);
            listView1.Columns.Add("Yön", 60);
            listView1.Columns.Add("Açıklama", 120);
        }
        public void ListViewElemanEkle(string aciklama)
        {
            int aralıkDegeri = (pictureBox1.Size.Width - 20) / Convert.ToInt32(HrtBytTextBox.Text);
            int konumX = (RobotKonumNow.X - 20) / aralıkDegeri;
            int konumY = (420-(20+aralıkDegeri) - RobotKonumNow.Y) / aralıkDegeri;
            ListViewItem item = new ListViewItem(hrktSayisi.ToString());
            item.SubItems.Add(konumX.ToString().ToUpper());
            item.SubItems.Add(konumY.ToString().ToUpper());
            item.SubItems.Add(RobotYönNow.ToString().ToUpper());
            if (hrktSayisi == 0)
            {
                item.SubItems.Add(aciklama);
            }
            else
            {
                item.SubItems.Add(aciklama); // Her adım için açıklama ekle
            }
            listView1.Items.Add(item); // ListView'e yeni satırı ekle
        }


        /// <summary>
        /// Robotu hareket ettirmek için girilen komutları işleyen ve robotu hareket ettiren butonun click eventıdır.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param> 
        int hrktSayisi = 0; // hareket sayısını tutan değişken
        private void HareketEttir_Click(object sender, EventArgs e)
        {
            if (listelemeDurumu == 1)
                hrktSayisi++;

            string aciklama;
            KomutTextBox.Text = KomutTextBox.Text.ToUpper();
            char[] Komutlar = KomutTextBox.Text.ToUpper().ToCharArray();

            if (KomutTextBox.Text != null)
            {
                foreach (char komut in Komutlar)
                {
                    if (komut == 'U') //UP
                    {
                        komutUP(RobotYönNow, Convert.ToInt32(HrtBytTextBox.Text));
                        if (listelemeDurumu == 1)
                        {
                            aciklama = "YUKARI İLERLEME-U";
                            ListViewElemanEkle(aciklama);
                        }
                    }
                    else if (komut == 'D')//DOWN
                    {
                        komutDOWN(RobotYönNow, Convert.ToInt32(HrtBytTextBox.Text));
                        if (listelemeDurumu == 1)
                        {
                            aciklama = "AŞAĞI İLERLEME-D";
                            ListViewElemanEkle(aciklama);
                        }
                    }
                    else if (komut == 'L')//LEFT
                    {
                        komutLEFT(RobotYönNow, Convert.ToInt32(HrtBytTextBox.Text));
                        if (listelemeDurumu == 1)
                        {
                            aciklama = "SOLA İLERLEME-L";
                            ListViewElemanEkle(aciklama);
                        }
                    }
                    else if(komut == 'R')//RIGHT
                    {
                        komutRIGHT(RobotYönNow, Convert.ToInt32(HrtBytTextBox.Text));
                        if (listelemeDurumu == 1)
                        {
                            aciklama = "SAĞA İLERLEME-R";
                            ListViewElemanEkle(aciklama);
                        }
                    }
                    else if (komut == 'T') // Saat yönünde 90 der döndür ve 1 br ileri git
                    {
                        komutTURN(RobotYönNow, Convert.ToInt32(HrtBytTextBox.Text));
                        if (listelemeDurumu == 1)
                        {
                            aciklama = "YÖN DEĞİŞTİRME VE İLERLEME";
                            ListViewElemanEkle(aciklama);
                        }
                    }
                    else if (komut == 'Y') // saat yönünün tersine 90 der gön ve 1 br ileri git
                    {
                        komutRETURN(RobotYönNow, Convert.ToInt32(HrtBytTextBox.Text));
                        if(listelemeDurumu == 1)
                        {
                            aciklama = "YÖN DEĞİŞTİRME VE İLERLEME";
                            ListViewElemanEkle(aciklama);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Lütfen bir komut giriniz.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
        }


        /// <summary>
        /// Geri butonuna tıklandığında robotun bir önceki konumuna geri dönmesini sağlayan fonksiyondur.
        /// </summary>
        int tıklama = 0;
        int kalinanindex=0;
        private void Geri_Click(object sender, EventArgs e)
        {
            Graphics g = pictureBox1.CreateGraphics();
            int aralıkDegeri = (pictureBox1.Size.Width - 20) / Convert.ToInt32(HrtBytTextBox.Text);
            if (tıklama == 0)
                tıklama++;
            else
                tıklama += 2;

            if (tıklama >= listView1.Items.Count)
            {
                MessageBox.Show("Geri alınacak adım kalmadı.", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Geri.Enabled = false; // Geri butonunu devre dışı bırak
                return;
            }
           
            int index = listView1.Items.Count - 1 - tıklama; // Son elemanı bul
            if ((index) >= 0)
            {
                g.FillEllipse(Brushes.White, new Rectangle(RobotKonumNow.X, RobotKonumNow.Y, aralıkDegeri, aralıkDegeri));

                ListViewItem sonEleman = listView1.Items[index];
                // Robotun konumunu ve yönünü geri al
                RobotKonumNow.X = Convert.ToInt32(sonEleman.SubItems[1].Text);
                RobotKonumNow.Y = Convert.ToInt32(sonEleman.SubItems[2].Text);
                RobotYönNow = sonEleman.SubItems[3].Text;

                // Robotun yeni konumunu çiz
                g.FillEllipse(new SolidBrush(Color.DarkBlue), new Rectangle(RobotKonumNow.X, RobotKonumNow.Y, aralıkDegeri, aralıkDegeri));
                Point merkez = new Point(RobotKonumNow.X + (aralıkDegeri / 2), RobotKonumNow.Y + (aralıkDegeri / 2)); //merkezi güncelle

                Pen kalem = new Pen(Color.Red, 2);
                if (RobotYönNow == "K") //Kuzey
                {
                    Point point = new Point(merkez.X, merkez.Y - aralıkDegeri / 2); // yukarı doğru yönü
                    g.DrawLine(kalem, merkez, point);
                    RobotYönNow = "K"; // Yönü Kuzey olarak güncelle
                }
                else if (RobotYönNow == "G") //Güney
                {
                    Point point = new Point(merkez.X, merkez.Y + aralıkDegeri / 2); // aşağı doğru yönü
                    g.DrawLine(kalem, merkez, point);
                    RobotYönNow = "G"; // Yönü Güney olarak güncelle
                }
                else if (RobotYönNow == "D") //Doğu
                {
                    Point point = new Point(merkez.X + (aralıkDegeri / 2), merkez.Y); // sağa doğru yönü
                    g.DrawLine(kalem, merkez, point);
                    RobotYönNow = "D"; // Yönü Doğu olarak güncelle
                }
                else if (RobotYönNow == "B") //Batı
                {
                    Point point = new Point(merkez.X - (aralıkDegeri / 2), merkez.Y); // sola doğru yönü
                    g.DrawLine(kalem, merkez, point);
                    RobotYönNow = "B"; // Yönü Batı olarak güncelle
                }
                else
                {
                    MessageBox.Show("Lütfen geçerli bir yön giriniz.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                hrktSayisi++;
                kalinanindex = Convert.ToInt32(sonEleman.SubItems[0].Text);
                ListViewElemanEkle("Geri alındı."); // Geri alındığını listview'e ekle
                listView1.Refresh(); // ListView'i güncelle
            }
            else
            {
                MessageBox.Show("Geri alınacak adım kalmadı.", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        /// <summary>
        /// İleri butonuna tıklandığında robotun bir sonraki konumuna ilerlemesini sağlayan fonksiyondur.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void İleri_Click(object sender, EventArgs e)
        {
            int gidilecekindex = kalinanindex + 1; // İleri butonuna tıklandığında bir sonraki indeksi hesapla
            if (gidilecekindex <= listView1.Items.Count - 1)
            {
                Graphics g = pictureBox1.CreateGraphics();
                int aralıkDegeri = (pictureBox1.Size.Width - 20) / Convert.ToInt32(HrtBytTextBox.Text);
                g.FillEllipse(Brushes.White, new Rectangle(RobotKonumNow.X, RobotKonumNow.Y, aralıkDegeri, aralıkDegeri));

                ListViewItem gidilenEleman = listView1.Items[gidilecekindex];
                RobotKonumNow.X = Convert.ToInt32(gidilenEleman.SubItems[1].Text);
                RobotKonumNow.Y = Convert.ToInt32(gidilenEleman.SubItems[2].Text);
                RobotYönNow = gidilenEleman.SubItems[3].Text;

                g.FillEllipse(new SolidBrush(Color.DarkBlue), new Rectangle(RobotKonumNow.X, RobotKonumNow.Y, aralıkDegeri, aralıkDegeri));
                Point merkez = new Point(RobotKonumNow.X + (aralıkDegeri / 2), RobotKonumNow.Y + (aralıkDegeri / 2)); //merkezi güncelle

                Pen kalem = new Pen(Color.Red, 2);
                if (RobotYönNow == "K") //Kuzey
                {
                    Point point = new Point(merkez.X, merkez.Y - aralıkDegeri / 2); // yukarı doğru yönü
                    g.DrawLine(kalem, merkez, point);
                    RobotYönNow = "K"; // Yönü Kuzey olarak güncelle
                }
                else if (RobotYönNow == "G") //Güney
                {
                    Point point = new Point(merkez.X, merkez.Y + aralıkDegeri / 2); // aşağı doğru yönü
                    g.DrawLine(kalem, merkez, point);
                    RobotYönNow = "G"; // Yönü Güney olarak güncelle
                }
                else if (RobotYönNow == "D") //Doğu
                {
                    Point point = new Point(merkez.X + (aralıkDegeri / 2), merkez.Y); // sağa doğru yönü
                    g.DrawLine(kalem, merkez, point);
                    RobotYönNow = "D"; // Yönü Doğu olarak güncelle
                }
                else if (RobotYönNow == "B") //Batı
                {
                    Point point = new Point(merkez.X - (aralıkDegeri / 2), merkez.Y); // sola doğru yönü
                    g.DrawLine(kalem, merkez, point);
                    RobotYönNow = "B"; // Yönü Batı olarak güncelle
                }
                else
                {
                    MessageBox.Show("Lütfen geçerli bir yön giriniz.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                hrktSayisi++;
                kalinanindex = Convert.ToInt32(gidilenEleman.SubItems[0].Text);
                ListViewElemanEkle("Geri alındı."); // Geri alındığını listview'e ekle
                listView1.Refresh(); // ListView'i güncelle
            }
            else
            {
                MessageBox.Show("İleri alınacak adım kalmadı.", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }
    }
}
