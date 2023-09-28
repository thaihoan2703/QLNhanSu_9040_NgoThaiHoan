using QLNhanSu.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace QLNhanSu
{
    public partial class frmPersonManagement : Form
    {
        public frmPersonManagement()
        {
            InitializeComponent();
        }


        private void frmPersonManagement_Load(object sender, EventArgs e)
        {
            PersonContextDB context = new PersonContextDB();
            List<NhanVien> listPersons = context.NhanViens.ToList();
            List<PhongBan> listFaculties = context.PhongBans.ToList();
            FillFacultyCombobox(listFaculties);
            BindGrid(listPersons);

        }
        private void FillFacultyCombobox(List<PhongBan> listFaculties)
        {
            this.cmbPhongBan.DataSource = listFaculties;
            this.cmbPhongBan.DisplayMember = "TenPB";
            this.cmbPhongBan.ValueMember = "MaPB";

        }

        private void BindGrid(List<NhanVien> listPersons)
        {
            dgvPerson.Rows.Clear();
            foreach (var item in listPersons)
            {
                int index = dgvPerson.Rows.Add();
                dgvPerson.Rows[index].Cells[0].Value = item.MaNV;
                dgvPerson.Rows[index].Cells[1].Value = item.TenNV;
                dgvPerson.Rows[index].Cells[2].Value = item.NgaySinh.ToString("dd/MM/yyyy");
                dgvPerson.Rows[index].Cells[3].Value = item.PhongBan.TenPB;
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtMaNV.Text == "" || txtTenNV.Text == "" || cmbPhongBan.Text == "")
                {
                    throw new Exception("Vui long nhap day du thong tin");
                }
                PersonContextDB context = new PersonContextDB();
                List<NhanVien> listStudents = context.NhanViens.ToList();

                NhanVien dbCheckID = context.NhanViens.FirstOrDefault(std => std.MaNV == txtMaNV.Text);

                if (txtMaNV.Text.Length != 6)
                {
                    txtMaNV.Focus();
                    throw new Exception("Vui long nhap ma so nv co 6 ki tu");

                }

                if (dbCheckID != null)
                {
                    throw new Exception("Ma so sv nay da ton tai");

                }
                string selectedFaculty = cmbPhongBan.Text;
                PhongBan selectedFacultyObj = context.PhongBans.FirstOrDefault(f => f.TenPB == selectedFaculty);
                string MaPB = selectedFacultyObj.MaPB;

                NhanVien s = new NhanVien() { MaNV = txtMaNV.Text, TenNV = txtTenNV.Text, NgaySinh = dtpNgaySinh.Value, MaPB = MaPB };
                context.NhanViens.Add(s);
                context.SaveChanges();

                // Load lại datagridview với danh sách nhan viên mới.
                List<NhanVien> listNewStudents = context.NhanViens.ToList();
                dgvPerson.DataSource = null;
                BindGrid(listNewStudents);
                // Clear the message.
                txtMaNV.Clear();
                txtTenNV.Clear();
                dtpNgaySinh.Value = DateTime.Now;

                throw new Exception("Them moi thanh cong!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Thong bao", MessageBoxButtons.OK);

            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                PersonContextDB context = new PersonContextDB();
                List<NhanVien> listStudents = context.NhanViens.ToList();

                string selectedFaculty = cmbPhongBan.Text;
                PhongBan selectedFacultyObj = context.PhongBans.FirstOrDefault(f => f.TenPB == selectedFaculty);
                string MaPB = selectedFacultyObj.MaPB;

                NhanVien dbUpdate = context.NhanViens.FirstOrDefault(std => std.MaNV == txtMaNV.Text);
                if (dbUpdate != null)
                {

                    dbUpdate.TenNV = txtTenNV.Text;
                    dbUpdate.MaPB = MaPB;
                    dbUpdate.NgaySinh = dtpNgaySinh.Value;

                    context.SaveChanges();

                    // Load lại datagridview với danh sách sinh viên mới.
                    List<NhanVien> listNewStudents = context.NhanViens.ToList();
                    dgvPerson.DataSource = null;
                    BindGrid(listNewStudents);
                    // Clear the message.
                    txtMaNV.Clear();
                    txtTenNV.Clear();

                    throw new Exception("Cap nhat thanh cong!");
                }
                else
                {
                    if (dbUpdate == null)
                    {
                        throw new Exception("Khong the thay doi ma so sv!");

                    }
                    throw new Exception("Cap nhat khong thanh cong!");


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Thong bao", MessageBoxButtons.OK);

            }
        }

        private void dgvPerson_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;
            try
            {
                if(dgvPerson.SelectedRows.Count >0 || dgvPerson.SelectedRows.Count < dgvPerson.RowCount)
                {
                    txtMaNV.Text = dgvPerson.Rows[index].Cells[0].Value.ToString();
                    txtTenNV.Text = dgvPerson.Rows[index].Cells[1].Value.ToString();

                    string ngaySinhString = dgvPerson.Rows[index].Cells[2].Value.ToString();
                    // Chuyển đổi chuỗi thành datetime theo định dạng "dd/MM/yyyy"
                    DateTime ngaySinh = DateTime.ParseExact(ngaySinhString, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    // Gán giá trị datetime cho datetimepicker
                    dtpNgaySinh.Value = ngaySinh;

                    cmbPhongBan.Text = dgvPerson.Rows[index].Cells[3].Value.ToString();
                }else
                    throw new Exception("Vui long chon lai hang");

            }catch(Exception ex)
            {
                // Hiển thị thông báo nhắc người dùng chọn lại
                MessageBox.Show(ex.Message);
            }
        }

        private void dgvPerson_SelectionChanged(object sender, EventArgs e)
        {
            
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                PersonContextDB context = new PersonContextDB();
                List<NhanVien> listStudents = context.NhanViens.ToList();
                DialogResult dl = MessageBox.Show("Ban co chac muon xoa nhan vien nay?", "Thong bao", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dl == DialogResult.Yes)
                {
                    NhanVien dbDelete = context.NhanViens.FirstOrDefault(std => std.MaNV == txtMaNV.Text);
                    if (dbDelete != null)
                    {
                        context.NhanViens.Remove(dbDelete);
                        context.SaveChanges();


                        // Load lại datagridview với danh sách nhân viên mới.
                        List<NhanVien> listNewStudents = context.NhanViens.ToList();
                        dgvPerson.DataSource = null;
                        BindGrid(listNewStudents);
                        // Clear the message.
                        txtMaNV.Clear();
                        txtTenNV.Clear();

                        throw new Exception("Xoa nhan vien thanh cong!");
                    }
                    else
                    {
                        throw new Exception("Vui long chon nhan vien can xoa!");

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult dl = MessageBox.Show("Ban co chac muon thoat ung dung khong?", "Thong bao", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dl == DialogResult.Yes)
            {
                this.Close();
            }
        }
    }
}
