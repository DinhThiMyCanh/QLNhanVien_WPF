using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Data.Linq;

namespace QLNhanVien_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NhanVienDataContext dc = new NhanVienDataContext();
        Table<DMPHONG> DMPHONGs;
        Table<CHUCVU> CHUCVUs;
        Table<NhanVien> NhanViens;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(Windows_Load);
            btnThem.Click += new RoutedEventHandler(ThemNV);
            btnSua.Click += new RoutedEventHandler(SuaNV);
            btnXoa.Click += new RoutedEventHandler(Xoa);
            DataGrid.SelectionChanged += new SelectionChangedEventHandler(Data_Click);

        }

        private void Data_Click(object sender, SelectionChangedEventArgs e)
        {
            NhanVien nv = DataGrid.SelectedItem as NhanVien; //Nhân viên được chọn trong DataGrid
            if (nv!=null)
            {
                txtMaNV.Text = nv.MaNV.ToString();
                txtHoTen.Text = nv.HoTen.ToString();
                dtpNgaySinh.Text = nv.NgaySinh.ToString();
                string gt = nv.GioiTinh.ToString();
                if (gt.Equals("True"))
                    rdNam.IsChecked = true;
                else
                    rdNu.IsChecked = true;
                txtSoDT.Text = nv.SoDT.ToString();
                txtHSL.Text = nv.HeSoLuong.ToString();
                cboTenPhong.SelectedValue = nv.MaPhong.ToString();
                cboChucVu.SelectedValue = nv.MaChucVu.ToString();
            }    
        }

        private void Xoa(object sender, RoutedEventArgs e)
        {
            var query = from nv in NhanViens
                        where nv.MaNV == int.Parse(txtMaNV.Text)
                        select nv;

            foreach (var nv in query)
            {
                NhanViens.DeleteOnSubmit(nv);
            }    

            //Cập nhật dữ liệu xuống Database
            dc.SubmitChanges();
           LoadNV();
        }

        private void SuaNV(object sender, RoutedEventArgs e)
        {
            int maNV = int.Parse(txtMaNV.Text);
            var nv = dc.NhanViens.SingleOrDefault(n => n.MaNV == maNV); //Truy vấn Linq theo phương thức

            if (nv != null)
            {
                nv.HoTen = txtHoTen.Text;
                nv.NgaySinh = Convert.ToDateTime(dtpNgaySinh.Text);
                nv.GioiTinh = rdNam.IsChecked == true;
                nv.SoDT = txtSoDT.Text;
                nv.HeSoLuong = float.Parse(txtHSL.Text);

                // Gán entity
                var phong = dc.DMPHONGs.SingleOrDefault(p => p.MaPhong == cboTenPhong.SelectedValue.ToString());
                var chucVu = dc.CHUCVUs.SingleOrDefault(c => c.MaChucVu == cboChucVu.SelectedValue.ToString());

                if (phong != null)
                    nv.DMPHONG = phong;

                if (chucVu != null)
                    nv.CHUCVU = chucVu;

                dc.SubmitChanges();
            }
            else
            {
                MessageBox.Show("Không tìm thấy nhân viên.");
            }

            LoadNV();

        }

        private void ThemNV(object sender, RoutedEventArgs e)
        {
            NhanVien nv = new NhanVien();
            nv.HoTen = txtHoTen.Text;
            nv.NgaySinh = Convert.ToDateTime(dtpNgaySinh.Text);
            nv.GioiTinh = rdNam.IsChecked == true ? true : false;
            nv.SoDT = txtSoDT.Text;
            nv.HeSoLuong = float.Parse(txtHSL.Text);
            nv.MaPhong = cboTenPhong.SelectedValue.ToString();
            nv.MaChucVu = cboChucVu.SelectedValue.ToString();

            //Thêm nv vào Entity NhanViens
            NhanViens.InsertOnSubmit(nv);
            //Cập nhật dữ liệu xuống Database
            dc.SubmitChanges();
            LoadNV();
        }

        private void Windows_Load(object sender, RoutedEventArgs e)
        {
            loadPB();
            LoadCV();
            LoadNV();
        }

        //Lấy dữ liệu Phòng ban lên cboPhongBan
        public void loadPB()
        {
            //Nguồn dữ liệu
            DMPHONGs = dc.GetTable<DMPHONG>();
            //Truy vấn Linq
            var query = from pb in DMPHONGs
                        select new { mapb = pb.MaPhong, tenpb = pb.TenPhong };
            //Thực thi truy vấn
            cboTenPhong.ItemsSource = query;
            cboTenPhong.DisplayMemberPath = "tenpb";
            cboTenPhong.SelectedValuePath = "mapb";

        }
        //Lấy dữ liệu chức vụ lên CboChucvu
        public void LoadCV()
        {
            //Nguồn dữ liệu
            CHUCVUs = dc.GetTable<CHUCVU>();
            //Truy vấnLinq
            var query = from cv in CHUCVUs
                        select new { maCV = cv.MaChucVu, tenCV = cv.TenChucVu };
            //Thực thi
            cboChucVu.ItemsSource = query;
            cboChucVu.DisplayMemberPath = "tenCV";
            cboChucVu.SelectedValuePath = "maCV";
        }
        //Load dữ liệu Nhân viên lên DataGrid
        public void LoadNV()
        {
            //Nguồn dữ liệu
            NhanViens = dc.GetTable<NhanVien>();
            //Truy vấn Linq
            var query = from nv in NhanViens
                        select nv;
            //Thực thi
            DataGrid.ItemsSource = query;
        }
    }
}
