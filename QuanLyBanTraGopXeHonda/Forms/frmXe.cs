using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuanLyBanTraGopXeHonda.Data;

namespace QuanLyBanTraGopXeHonda.Forms
{
    public partial class frmXe : Form
    {
        QLBXDbContext context = new QLBXDbContext();
        bool xuLyThem = false;
        int id;
        string imagesFolder = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory
                .Replace("bin\\Debug\\net9.0-windows\\", "")
                .Replace("bin\\Release\\net9.0-windows\\", ""),
            "Images");

        public frmXe()
        {
            InitializeComponent();
        }

        private void BatTatChucNang(bool giaTri)
        {
            btnLuu.Enabled = giaTri;
            btnHuyBo.Enabled = giaTri;
            cboHangXe.Enabled = giaTri;
            cboPhanLoai.Enabled = giaTri;
            txtTenXe.Enabled = giaTri;
            numSoLuong.Enabled = giaTri;
            numGiaBan.Enabled = giaTri;
            txtMoTa.Enabled = giaTri;
            picHinhAnh.Enabled = giaTri;
            btnDoiAnh.Enabled = giaTri;

            btnThem.Enabled = !giaTri;
            btnSua.Enabled = !giaTri;
            btnXoa.Enabled = !giaTri;
            btnTimKiem.Enabled = !giaTri;
            btnNhap.Enabled = !giaTri;
            btnXuat.Enabled = !giaTri;
        }

        public void LayLoaiXeVaoComboBox()
        {
            cboPhanLoai.DataSource = context.LoaiXes.ToList();
            cboPhanLoai.ValueMember = "ID";
            cboPhanLoai.DisplayMember = "TenLX";
        }

        public void LayHangXeVaoComboBox()
        {
            cboHangXe.DataSource = context.HangXes.ToList();
            cboHangXe.ValueMember = "ID";
            cboHangXe.DisplayMember = "TenHX";
        }

        private void frmXe_Load(object sender, EventArgs e)
        {
            BatTatChucNang(false);
            LayLoaiXeVaoComboBox();
            LayHangXeVaoComboBox();
            dataGridView1.AutoGenerateColumns = false;

            List<DanhSachXe> dsXe = context.Xes.Select(r => new DanhSachXe
            {
                ID = r.ID,
                LoaiXeID = r.LoaiXeID,
                TenLX = r.LoaiXes.TenLX,
                HangXeID = r.HangXeID,
                TenHX = r.HangXes.TenHX,
                TenXe = r.TenXe,
                GiaBan = r.GiaBan,
                SoLuong = r.SoLuong,
                HinhAnh = r.HinhAnh,
                MoTa = r.MoTa
            }).ToList();

            BindingSource bindingSource = new BindingSource();
            bindingSource.DataSource = dsXe;

            cboPhanLoai.DataBindings.Clear();
            cboPhanLoai.DataBindings.Add("SelectedValue", bindingSource, "LoaiXeID", false, DataSourceUpdateMode.Never);

            cboHangXe.DataBindings.Clear();
            cboHangXe.DataBindings.Add("SelectedValue", bindingSource, "HangXeID", false, DataSourceUpdateMode.Never);

            txtTenXe.DataBindings.Clear();
            txtTenXe.DataBindings.Add("Text", bindingSource, "TenXe", false, DataSourceUpdateMode.Never);

            txtMoTa.DataBindings.Clear();
            txtMoTa.DataBindings.Add("Text", bindingSource, "MoTa", false, DataSourceUpdateMode.Never);

            numSoLuong.DataBindings.Clear();
            numSoLuong.DataBindings.Add("Value", bindingSource, "SoLuong", false, DataSourceUpdateMode.Never);

            numGiaBan.DataBindings.Clear();
            numGiaBan.DataBindings.Add("Value", bindingSource, "GiaBan", false, DataSourceUpdateMode.Never);

            picHinhAnh.DataBindings.Clear();
            Binding hinhAnh = new Binding("ImageLocation", bindingSource, "HinhAnh");
            hinhAnh.Format += (s, ev) =>
            {
                if (ev.Value is string fileName && !string.IsNullOrEmpty(fileName))
                    ev.Value = Path.Combine(imagesFolder, fileName);
                else
                    ev.Value = null;
            };
            picHinhAnh.DataBindings.Add(hinhAnh);

            dataGridView1.DataSource = bindingSource;
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "HinhAnh")
            {
                if (e.Value is string fileName && !string.IsNullOrEmpty(fileName))
                {
                    string imagePath = Path.Combine(imagesFolder, fileName);
                    if (File.Exists(imagePath))
                    {
                        Image image = Image.FromFile(imagePath);
                        image = new Bitmap(image, 24, 24);
                        e.Value = image;
                    }
                    else
                    {
                        e.Value = null;
                    }
                }
                else
                {
                    e.Value = null;
                }
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            xuLyThem = true;
            BatTatChucNang(true);
            cboPhanLoai.SelectedIndex = -1;
            cboHangXe.SelectedIndex = -1;
            txtTenXe.Clear();
            txtMoTa.Clear();
            numSoLuong.Value = 0;
            numGiaBan.Value = 0;
            picHinhAnh.Image = null;
            picHinhAnh.ImageLocation = null;
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Vui long chon mot xe de sua.", "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            xuLyThem = false;
            id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["ID"].Value);
            BatTatChucNang(true);
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (cboPhanLoai.SelectedValue == null || string.IsNullOrWhiteSpace(cboPhanLoai.Text))
                MessageBox.Show("Vui long chon loai xe.", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (cboHangXe.SelectedValue == null || string.IsNullOrWhiteSpace(cboHangXe.Text))
                MessageBox.Show("Vui long chon hang xe.", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (string.IsNullOrWhiteSpace(txtTenXe.Text))
                MessageBox.Show("Vui long nhap ten xe.", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (numSoLuong.Value <= 0)
                MessageBox.Show("So luong phai lon hon 0.", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (numGiaBan.Value <= 0)
                MessageBox.Show("Gia ban phai lon hon 0.", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                if (xuLyThem)
                {
                    Xe xe = new Xe()
                    {
                        TenXe = txtTenXe.Text.Trim(),
                        HangXeID = (int)cboHangXe.SelectedValue,
                        LoaiXeID = (int)cboPhanLoai.SelectedValue,
                        SoLuong = (int)numSoLuong.Value,
                        GiaBan = numGiaBan.Value,
                        HinhAnh = picHinhAnh.ImageLocation != null
                            ? Path.GetFileName(picHinhAnh.ImageLocation)
                            : null,
                        MoTa = txtMoTa.Text.Trim()
                    };
                    context.Xes.Add(xe);
                    context.SaveChanges();
                }
                else
                {
                    Xe? xe = context.Xes.Find(id);
                    if (xe != null)
                    {
                        xe.TenXe = txtTenXe.Text.Trim();
                        xe.HangXeID = (int)cboHangXe.SelectedValue;
                        xe.LoaiXeID = (int)cboPhanLoai.SelectedValue;
                        xe.SoLuong = (int)numSoLuong.Value;
                        xe.GiaBan = numGiaBan.Value;
                        xe.MoTa = txtMoTa.Text.Trim();
                        context.Xes.Update(xe);
                        context.SaveChanges();
                    }
                }
                frmXe_Load(sender, e);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Vui long chon mot xe de xoa.", "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string tenXe = txtTenXe.Text;
            if (MessageBox.Show("Xac nhan xoa xe \"" + tenXe + "\"?", "Xoa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["ID"].Value);
                Xe? xe = context.Xes.Find(id);
                if (xe != null)
                {
                    context.Xes.Remove(xe);
                    context.SaveChanges();
                }
                frmXe_Load(sender, e);
            }
        }

        private void btnHuyBo_Click(object sender, EventArgs e)
        {
            frmXe_Load(sender, e);
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDoiAnh_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Cap nhat hinh anh xe";
            openFileDialog.Filter = "Tap tin hinh anh|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                string ext = Path.GetExtension(openFileDialog.FileName);
                string savedFileName = fileName + ext;
                string fileSavePath = Path.Combine(imagesFolder, savedFileName);

                Directory.CreateDirectory(imagesFolder);
                File.Copy(openFileDialog.FileName, fileSavePath, true);

                if (xuLyThem)
                {
                    picHinhAnh.ImageLocation = fileSavePath;
                }
                else if (dataGridView1.CurrentRow != null)
                {
                    id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["ID"].Value);
                    Xe? xe = context.Xes.Find(id);
                    if (xe != null)
                    {
                        xe.HinhAnh = savedFileName;
                        context.Xes.Update(xe);
                        context.SaveChanges();
                        frmXe_Load(sender, e);
                    }
                }
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chuc nang tim kiem dang duoc phat trien.", "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnNhap_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chuc nang nhap dang duoc phat trien.", "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnXuat_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chuc nang xuat dang duoc phat trien.", "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
