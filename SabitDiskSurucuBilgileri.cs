// Burada Disk Bilgilerinde İhtiyaç Duyduğum Bilgileri Kullanabilmek İçin Nesne Tabanlı Bir Class Tanımlanmaktadır, Siz Kendi İhtiyaçlarınıza Göre Düzenleyebilirsiniz 
// Referans Olarak Tanımlanması

using System.Management;

// ManagementObjectSearcher İle Disk Sürücleri Listeleme
var DiskSurucuSorgulama = new ManagementObjectSearcher("Select * From Win32_DiskDrive");

// Listelenen Sürücüleri foreach İle Döngü İçerisine Dahil Edilmesi
foreach (ManagementObject Nesne1 in DiskSurucuSorgulama.Get())
{
    // Disk Sürücüleri Partition Bilgisini Kontrol Edilmesi
    var BolumSorguMetni = string.Format("associators of {{{0}}} where AssocClass = Win32_DiskDriveToDiskPartition", Nesne1.Path.RelativePath);
    var BolumSorgusu = new ManagementObjectSearcher(BolumSorguMetni);

    // Listelenen Sürücüleri Bölüm Sorgusu İçin foreach İle Döngü İçerisine Dahil Edilmesi
    foreach (ManagementObject Nesne2 in BolumSorgusu.Get())
    {
        // Yerel Disk Sürücüleri Partition Bilgisini Kontrol Edilmesi
        var MantiksalSurucuSorguMetni = string.Format("associators of {{{0}}} where AssocClass = Win32_LogicalDiskToPartition", Nesne2.Path.RelativePath);
        var MantiksalSurucuSorgusu = new ManagementObjectSearcher(MantiksalSurucuSorguMetni);

        // Listelenen Sürücüleri Mantiksal Bölüm Sorgusu foreach İle Döngü İçerisine Dahil Edilmesi
        foreach (ManagementObject Nesne3 in MantiksalSurucuSorgusu.Get())
        {
            // MediaType Türüne Göre Kontrol Sağlanması
            if (Convert.ToString(Nesne1.Properties["MediaType"].Value) == "Fixed hard disk media")
            {
                // Oluşturduğumuz Class İçerisine Kullanacağımız Disk Bilgilerinin Aktarılması
                BirimListesi.Add(new Yapilandirma.YerelDiskBirimleriListesi
                {
                    SurucuAdi = Convert.ToString(Nesne3.Properties["Name"].Value) + "\\",
                    DosyaSistemi = Convert.ToString(Nesne3.Properties["FileSystem"].Value),
                    BosAlan = Convert.ToUInt64(Nesne3.Properties["FreeSpace"].Value)
                });
            }
        }
    }
}