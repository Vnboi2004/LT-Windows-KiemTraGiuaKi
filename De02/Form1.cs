using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using De02.Model; // Namespace chứa DbContext

namespace De02
{
    public partial class Form1 : Form
    {
        // Tạo một instance của DbContext
        private QuanLySanPhamContextDB db = new QuanLySanPhamContextDB();
        private bool isAdding = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Vô hiệu hóa nút "Lưu" và "Không Lưu" khi tải form
            btn_Luu.Enabled = false;
            btn_K_Luu.Enabled = false;

            // Khóa các ô nhập liệu
            EnableInputFields(false);

            // Tải dữ liệu sản phẩm vào DataGridView
            LoadDataToDataGridView();

            // Tải danh sách loại sản phẩm vào ComboBox
            LoadLoaiSanPhamToComboBox();
        }

        private void LoadDataToDataGridView()
        {
            try
            {
                // Lấy dữ liệu từ bảng SanPhams và hiển thị tên loại sản phẩm
                var sanPhams = db.SanPhams
                                .Join(db.LoaiSPs,
                                      sp => sp.MaLoai,
                                      lsp => lsp.MaLoai,
                                      (sp, lsp) => new
                                      {
                                          MaSP = sp.MaSP,
                                          TenSP = sp.TenSP,
                                          NgayNhap = sp.NgayNhap,
                                          TenLoai = lsp.TenLoai // Hiển thị tên loại sản phẩm
                                      })
                                .ToList();

                // Gán dữ liệu vào DataGridView
                dgvSanpham.DataSource = sanPhams;

                // Tùy chỉnh tiêu đề cột
                dgvSanpham.Columns[0].HeaderText = "Mã SP";
                dgvSanpham.Columns[1].HeaderText = "Tên Sản Phẩm";
                dgvSanpham.Columns[2].HeaderText = "Ngày Nhập";
                dgvSanpham.Columns[3].HeaderText = "Loại Sản Phẩm"; // Hiển thị tên loại
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message);
            }
        }

        private void LoadLoaiSanPhamToComboBox()
        {
            try
            {
                // Lấy danh sách loại sản phẩm
                var loaiSPs = db.LoaiSPs
                                .Select(lsp => new
                                {
                                    MaLoai = lsp.MaLoai,
                                    TenLoai = lsp.TenLoai
                                })
                                .ToList();

                // Gán dữ liệu vào ComboBox
                cboLoaiSP.DataSource = loaiSPs;
                cboLoaiSP.DisplayMember = "TenLoai"; // Hiển thị tên loại
                cboLoaiSP.ValueMember = "MaLoai";   // Lấy giá trị mã loại
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải loại sản phẩm: " + ex.Message);
            }
        }


        private void btn_Them_Click(object sender, EventArgs e)
        {
            // Bật trạng thái thêm sản phẩm
            isAdding = true;

            // Mở khóa các ô nhập liệu
            EnableInputFields(true);

            // Bật nút "Lưu" và "Không Lưu"
            btn_Luu.Enabled = true;
            btn_K_Luu.Enabled = true;

            // Xóa trắng các trường nhập liệu
            ClearInputFields();

            // Đặt focus vào trường nhập liệu đầu tiên
            txt_Ma_SP.Focus();
        }

        private void btn_Luu_Click(object sender, EventArgs e)
        {
            // Xử lý xóa sản phẩm
            if (selectedMaSPs.Any())
            {
                try
                {
                    foreach (string maSP in selectedMaSPs)
                    {
                        // Lấy sản phẩm từ cơ sở dữ liệu dựa trên mã sản phẩm
                        var sanPham = db.SanPhams.FirstOrDefault(sp => sp.MaSP == maSP);
                        if (sanPham != null)
                        {
                            // Xóa sản phẩm
                            db.SanPhams.Remove(sanPham);
                        }
                    }

                    // Lưu thay đổi vào cơ sở dữ liệu
                    db.SaveChanges();

                    MessageBox.Show("Xóa sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Tải lại dữ liệu vào DataGridView
                    LoadDataToDataGridView();

                    // Đặt lại danh sách mã sản phẩm đã chọn
                    selectedMaSPs.Clear();

                    // Đặt lại trạng thái
                    ResetState();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa sản phẩm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (isAdding)
            {
                // Xử lý thêm sản phẩm
                try
                {
                    SanPham sp = new SanPham
                    {
                        MaSP = txt_Ma_SP.Text.Trim(),
                        TenSP = txt_Ten_SP.Text.Trim(),
                        NgayNhap = dtpNgayNhap.Value,
                        MaLoai = cboLoaiSP.SelectedValue.ToString()
                    };

                    db.SanPhams.Add(sp);
                    db.SaveChanges();

                    MessageBox.Show("Thêm sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDataToDataGridView();
                    ResetState();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi thêm sản phẩm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (!string.IsNullOrEmpty(txt_Ma_SP.Text))
            {
                // Xử lý sửa sản phẩm
                try
                {
                    var sanPham = db.SanPhams.FirstOrDefault(sp => sp.MaSP == txt_Ma_SP.Text.Trim());

                    if (sanPham != null)
                    {
                        sanPham.TenSP = txt_Ten_SP.Text.Trim();
                        sanPham.NgayNhap = dtpNgayNhap.Value;
                        sanPham.MaLoai = cboLoaiSP.SelectedValue.ToString();

                        db.SaveChanges();

                        MessageBox.Show("Sửa sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadDataToDataGridView();
                        ResetState();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy sản phẩm để sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi sửa sản phẩm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }




        private void btn_K_Luu_Click(object sender, EventArgs e)
        {
            // Hủy bỏ hành động sửa
            if (!string.IsNullOrEmpty(txt_Ma_SP.Text))
            {
                MessageBox.Show("Đã hủy bỏ hành động sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // Đặt lại trạng thái
            ResetState();
        }



        private void ResetState()
        {
            isAdding = false;

            // Khóa các ô nhập liệu
            EnableInputFields(false);

            // Vô hiệu hóa các nút "Lưu" và "Không Lưu"
            btn_Luu.Enabled = false;
            btn_K_Luu.Enabled = false;

            // Xóa trắng các trường nhập liệu
            ClearInputFields();
        }



        private void EnableInputFields(bool isEnabled)
        {
            // Mở khóa hoặc khóa các trường nhập liệu
            txt_Ma_SP.Enabled = isEnabled;
            txt_Ten_SP.Enabled = isEnabled;
            dtpNgayNhap.Enabled = isEnabled;
            cboLoaiSP.Enabled = isEnabled;
        }

        private void ClearInputFields()
        {
            // Xóa trắng các trường nhập liệu
            txt_Ma_SP.Text = "";
            txt_Ten_SP.Text = "";
            dtpNgayNhap.Value = DateTime.Now;
            cboLoaiSP.SelectedIndex = -1; // Bỏ chọn ComboBox
        }


        private List<string> selectedMaSPs = new List<string>(); // Lưu mã sản phẩm cần xóa

        private void btn_Xoa_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra xem có dòng nào được chọn hay không
                if (dgvSanpham.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn ít nhất một dòng để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Lưu danh sách mã sản phẩm được chọn
                selectedMaSPs.Clear();
                foreach (DataGridViewRow row in dgvSanpham.SelectedRows)
                {
                    string maSP = row.Cells[0].Value.ToString();
                    selectedMaSPs.Add(maSP);
                }

                // Bật nút "Lưu" và "Không Lưu"
                btn_Luu.Enabled = true;
                btn_K_Luu.Enabled = true;

                // Thông báo để người dùng xác nhận hành động
                MessageBox.Show(
                    "Vui lòng nhấn 'Lưu' để xác nhận xóa hoặc 'Không Lưu' để hủy bỏ hành động.",
                    "Xác nhận hành động",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi chọn sản phẩm để xóa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_Sua_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra xem có dòng nào được chọn hay không
                if (dgvSanpham.SelectedRows.Count != 1)
                {
                    MessageBox.Show("Vui lòng chọn một dòng để sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Lấy dòng được chọn
                DataGridViewRow selectedRow = dgvSanpham.SelectedRows[0];

                // Hiển thị thông tin lên các trường nhập liệu
                txt_Ma_SP.Text = selectedRow.Cells[0].Value.ToString();
                txt_Ten_SP.Text = selectedRow.Cells[1].Value.ToString();
                dtpNgayNhap.Value = Convert.ToDateTime(selectedRow.Cells[2].Value);
                cboLoaiSP.Text = selectedRow.Cells[3].Value.ToString();

                // Bật các trường nhập liệu để chỉnh sửa
                EnableInputFields(true);

                // Bật nút "Lưu" và "Không Lưu"
                btn_Luu.Enabled = true;
                btn_K_Luu.Enabled = true;

                // Vô hiệu hóa việc chỉnh sửa mã sản phẩm (khóa mã sản phẩm)
                txt_Ma_SP.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi chọn dòng để sửa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_Thoat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
        "Bạn có chắc chắn muốn thoát không?",
        "Xác nhận thoát",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question
    );

            // Kiểm tra lựa chọn của người dùng
            if (result == DialogResult.Yes)
            {
                // Thoát khỏi ứng dụng
                Application.Exit();
            }
        }

        private void btn_Tim_Kiem_Click(object sender, EventArgs e)
        {
            string keyword = txt_Tim_Kiem.Text.Trim();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                MessageBox.Show("Vui lòng nhập tên sản phẩm cần tìm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var sanPhams = db.SanPhams
                                 .Where(sp => sp.TenSP.Contains(keyword))
                                 .Join(db.LoaiSPs,
                                       sp => sp.MaLoai,
                                       lsp => lsp.MaLoai,
                                       (sp, lsp) => new
                                       {
                                           MaSP = sp.MaSP,
                                           TenSP = sp.TenSP,
                                           Ngaynhap = sp.NgayNhap,
                                           TenLoai = lsp.TenLoai
                                       })
                                 .ToList();

                if (sanPhams.Any())
                {
                    dgvSanpham.DataSource = sanPhams; // Hiển thị kết quả tìm kiếm
                }
                else
                {
                    MessageBox.Show("Không tìm thấy sản phẩm phù hợp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tìm kiếm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
 

}
