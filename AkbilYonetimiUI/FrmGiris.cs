using AkbilYntmIsKatmani;
using AkbilYntmVeriKatmani;
using System.Data.SqlClient;
using System.Text;

namespace AkbilYonetimiUI
{
    public partial class FrmGiris : Form
    {
        public string Email { get; set; } // Kayıt ol formunda kayır olan kullanıcının emaili buraya gelsin

        IVeriTabaniIslemleri veriTabaniIslemleri = new SQLVeriTabaniIslemleri();
        public FrmGiris()
        {
            InitializeComponent();
        }

        private void FrmGiris_Load(object sender, EventArgs e)
        {
            if (Email != null)
            {
                txtEmail.Text = Email;

            }
            txtEmail.TabIndex = 1;
            txtSifre.TabIndex = 2;
            checkBoxHatirla.TabIndex = 3;
            btnGirisYap.TabIndex = 4;
            btnKayitOl.TabIndex = 5;
            txtSifre.PasswordChar = '*';

            //beni hatirlayi Properties.Settings ile yapana kadar burası böyle kolaylık saglasin
            txtEmail.Text = "maideseymaersahan@gmail.com";
            txtSifre.Text = "123456";


        }

        private void btnKayitOl_Click(object sender, EventArgs e)
        {
            // Bu formu gizleyeceğiz.
            // Kayıt ol formunu açacağız.
            this.Hide();
            FrmKayitOl frm = new FrmKayitOl();
            frm.Show();
        }

        private void btnGirisYap_Click(object sender, EventArgs e)
        {
            GirisYap();
        }

        private void GirisYap()
        {
            try

            {
                // 1) Email ve şifre textboxları dolu mu?
                if (string.IsNullOrEmpty(txtEmail.Text) || string.IsNullOrEmpty(txtSifre.Text))
                {
                    MessageBox.Show("Bilgileri eksiksiz giriniz!",
                        "UYARI", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                // 2) Girdiği email ve şifre veritabanında mevcur mu?
                // select * from Kullanicilar where Email= '' and Sifre=''

                string[] istedigimKolonlar = new string[] { "Id", "Ad", "Soyad" };
                string kosullar = string.Empty;
                StringBuilder sb = new StringBuilder();
                sb.Append($"Email='{txtEmail.Text.Trim()}'");
                sb.Append(" and ");
                sb.Append($"Parola='{GenelIslemler.MD5Encryption(txtSifre.Text.Trim())}'");
                kosullar = sb.ToString();

                var sonuc = veriTabaniIslemleri.VeriOku("Kullanicilar", istedigimKolonlar, kosullar);

                if (sonuc.Count == 0)
                {
                    MessageBox.Show("Email ya da şifre yanlış! Tekrar dene!");
                }
                else
                {
                    GenelIslemler.GirisYapanKullaniciID = (int)sonuc["Id"];
                    GenelIslemler.GirisYapanKullaniciAdSoyad = $"{sonuc["Ad"]} {sonuc["Soyad"]}";
                    MessageBox.Show($"Hoşgeldiniz....{GenelIslemler.GirisYapanKullaniciAdSoyad}");


                    // BENİ HATIRLA yazılacak
                    this.Hide();
                    FrmAnasayfa frmanasayfa = new FrmAnasayfa();
                    frmanasayfa.Show();


                }




            }
            catch (Exception hata)
            {
                // DipNot exceptionlar asla kullanıcıya gösterilmez
                // Exceptionlar loglanır. Biz şu an öğrenme / geliştirme aşamasında olduğumuz için yazdık.
                MessageBox.Show("Beklenmedik bir sorun oluştu!" + hata.Message);
            }
        }

        private void checkBoxHatirla_CheckedChanged(object sender, EventArgs e)
        {
            BeniHatirla();
        }

        private void BeniHatirla()
        {


        }

        private void txtSifre_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter)) // basılan tuş enter ise griş yapacak


            {
                GirisYap();
            }
        }

        private void txtSifre_TextChanged(object sender, EventArgs e)
        {

        }
    }
}