using AkbilYntmIsKatmani;
using AkbilYntmVeriKatmani;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkbilYntmVeriKatmani
{
    public class SQLVeriTabaniIslemleri : IVeriTabaniIslemleri
    {
        public string BaglantiCumlesi { get; set; }

        
        private SqlConnection baglanti;
        private SqlCommand komut;

        public SQLVeriTabaniIslemleri()
        {
            BaglantiCumlesi = GenelIslemler.SinifSQLBaglantiCumlesi;//buranin icine gotodefinition yapip kendi pc adini yaz
            baglanti = new SqlConnection(BaglantiCumlesi);
            komut = new SqlCommand();
            komut.Connection = baglanti; ;

        }

        public SQLVeriTabaniIslemleri(string baglantiCumle)
        {//burada baglanti cümesini parametreye atadık iki türlü kullanılabilir
            BaglantiCumlesi = baglantiCumle;
            baglanti = new SqlConnection(BaglantiCumlesi);
            komut = new SqlCommand();
            komut.Connection = baglanti;
        }


        private void BaglantiyiAc()
        {
            if (baglanti.State != ConnectionState.Open)
            {
                baglanti.Open();
            }
        }

        public int KomutIsle(string eklemeyadaGuncellemeCumlesi)
        {
            try
            {
                using (baglanti)
                {
                    baglanti.ConnectionString = BaglantiCumlesi;
                    komut.CommandType = CommandType.Text;
                    komut.CommandText = eklemeyadaGuncellemeCumlesi;
                    BaglantiyiAc();
                    int etkilenenSatirSayisi = komut.ExecuteNonQuery();
                    return etkilenenSatirSayisi;

                    // return komut.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string VeriEklemeCumlesiOlustur(string tabloAdi, Dictionary<string, object> kolonlar)
        {

            try
            {
                // insert into TabloAdi (kolonlar) values (degerler)
                string sorgu = string.Empty;
                string sutunlar = string.Empty;
                string degerler = string.Empty;

                foreach (var item in kolonlar)
                {
                    sutunlar += $"{item.Key},";
                    degerler += $"{item.Value},";
                }

                // en sondaki virgülden kurtulmamız için TRIM kullanalım.
                sutunlar = sutunlar.TrimEnd(',');
                degerler = degerler.TrimEnd(',');

                sorgu = $"insert into {tabloAdi} ({sutunlar}) values ({degerler})";
                return sorgu;

            }
            catch (Exception)
            {

                throw;
            }

        }

        public string VeriGuncellemeCumlesiOlustur(string tabloAdi, Hashtable kolonlar, string? kosullar = null)
        {
            try
            {
                //update TabloAdi set col1=deger1,... where kosul
                string sorgu = string.Empty, setler = string.Empty;
                foreach (var item in kolonlar.Keys)
                {
                    setler += $"{item}={kolonlar[item]}, ";
                }
                setler = setler.Trim().TrimEnd(','); //todo deneyecegiz...

                sorgu = $"update {tabloAdi} set {setler} ";
                if (!string.IsNullOrEmpty(kosullar))
                {
                    sorgu += $" where {kosullar}";
                }
                return sorgu;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Hashtable VeriOku(string tabloAdi, string[] kolonlar, string? kosullar = null)
        {
            try
            {
                Hashtable sonuc = new Hashtable();
                string sutunlar = string.Empty;
                //kolonlara , ekleyecegiz

               StringBuilder sb = new StringBuilder();
                foreach (var item in kolonlar)
                {
                    sb.Append($"{item},");

                }
                sutunlar = sb.ToString().TrimEnd(',');
                string sorgu = $"select {sutunlar} from {tabloAdi}";
                if (!string.IsNullOrEmpty(kosullar))
                {
                    sorgu += $" where {kosullar}";

                }

                using (baglanti)
                {
                    baglanti.ConnectionString = BaglantiCumlesi;
                    komut.CommandText = sorgu;
                    BaglantiyiAc();
                    SqlDataReader okuyucu = komut.ExecuteReader();
                    if (okuyucu.HasRows)
                    {
                        while (okuyucu.Read())
                        {
                            //for (int i = 0; i < kolonlar.Length; i++)
                            //{
                            //    sonuc.Add(kolonlar[i], okuyucu[kolonlar[i]]);

                            //}

                            foreach (var item in kolonlar)
                            {
                                sonuc.Add(item, okuyucu[item]);//birden cokakbilgetirdiginde??

                            }

                        }//while bitti.
                    }
                }


                return sonuc;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public int VeriSil(string tabloAdi, string? kosullar = null)
        {
            try
            {
                using (baglanti)
                {
                    baglanti.ConnectionString = BaglantiCumlesi;
                    string sorgu = $"delete from {tabloAdi} ";
                    if (!string.IsNullOrEmpty(kosullar))
                    {
                        sorgu += $" where {kosullar}";
                    }
                    komut.CommandText = sorgu;
                    BaglantiyiAc();
                    int silinenSatirSayisi = komut.ExecuteNonQuery();
                    return silinenSatirSayisi;

                    // return komut.ExecuteNonQUery();
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataTable VeriGetir(string tabloAdi, string kolonlar = "*", string? kosullar = null)
        {
            try
            {
                using (baglanti)
                {
                    baglanti.ConnectionString = BaglantiCumlesi;
                    string sorgu = $"select {kolonlar} from {tabloAdi}";
                    if (!string.IsNullOrEmpty(kosullar))
                    {
                        sorgu += $" where {kosullar}";
                    }
                    komut.CommandText = sorgu;
                    SqlDataAdapter adp = new SqlDataAdapter(komut);
                    BaglantiyiAc();
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    return dt;

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

       
    }
}