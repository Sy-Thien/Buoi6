using Lab04.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab04
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializeUIComponents();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                Model1 context = new Model1();
                List<Faculty> listFalcultys = context.Faculties.ToList(); //lấy các khoa
                List<Student> listStudent = context.Students.ToList(); //lấy sinh viên
                FillFalcultyCombobox(listFalcultys);
                BindGrid(listStudent);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void FillFalcultyCombobox(List<Faculty> listFalcultys)
        {
            this.cmbFaculty.DataSource = listFalcultys;
            this.cmbFaculty.DisplayMember = "FacultyName";
            this.cmbFaculty.ValueMember = "FacultyID";
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn thoát chương trình?",
                                      "Xác nhận Thoát",
                                      MessageBoxButtons.YesNo,
                                      MessageBoxIcon.Question);

            // Nếu người dùng chọn "Yes", thoát ứng dụng
            if (confirmResult == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
        private void BindGrid(List<Student> listStudent)
        {
            dgvStudent.Rows.Clear();
            foreach (var item in listStudent)
            {
                int index = dgvStudent.Rows.Add();
                dgvStudent.Rows[index].Cells[0].Value = item.StudentID;
                dgvStudent.Rows[index].Cells[1].Value = item.FullName;
                dgvStudent.Rows[index].Cells[2].Value = item.Faculty.FacultyName;
                dgvStudent.Rows[index].Cells[3].Value = item.AverageScore;
            }
        }

        private void dgvStudent_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0) // Đảm bảo không click vào tiêu đề
                {
                    txtStudentId.Text = dgvStudent.Rows[e.RowIndex].Cells[0].Value?.ToString();
                    txtFullname.Text = dgvStudent.Rows[e.RowIndex].Cells[1].Value?.ToString();
                    cmbFaculty.Text = dgvStudent.Rows[e.RowIndex].Cells[2].Value?.ToString();
                    txtAverageScore.Text = dgvStudent.Rows[e.RowIndex].Cells[3].Value?.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi chọn dòng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                // Tạo đối tượng context kết nối với cơ sở dữ liệu
                using (var context = new Model1())
                {
                    // Lấy danh sách sinh viên từ cơ sở dữ liệu
                    var studentList = context.Students.ToList();

                    // Kiểm tra nếu mã sinh viên đã tồn tại
                    if (studentList.Any(s => s.StudentID == txtStudentId.Text))
                    {
                        MessageBox.Show("Mã sinh viên đã tồn tại. Vui lòng nhập mã khác.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Tạo đối tượng sinh viên mới
                    var newStudent = new Student
                    {
                        StudentID = txtStudentId.Text,
                        FullName = txtFullname.Text,
                        AverageScore = double.Parse(txtAverageScore.Text),
                        FacultyID = int.Parse(cmbFaculty.SelectedValue.ToString())
                    };

                    // Thêm sinh viên mới vào cơ sở dữ liệu
                    context.Students.Add(newStudent);
                    context.SaveChanges();

                    // Tải lại dữ liệu vào DataGridView
                    BindGrid(context.Students.ToList());

                    // Hiển thị thông báo thành công
                    MessageBox.Show("Thêm sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và hiển thị thông báo lỗi
                MessageBox.Show($"Đã xảy ra lỗi khi thêm dữ liệu: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            btnCalculateTotal_Click(sender, e);
        }

        private void btnsua_Click(object sender, EventArgs e)
        {
            try
            {
                // Tạo đối tượng context kết nối với cơ sở dữ liệu
                using (var context = new Model1())
                {
                    // Tìm sinh viên cần chỉnh sửa dựa trên StudentID
                    var student = context.Students.FirstOrDefault(s => s.StudentID == txtStudentId.Text);

                    if (student != null)
                    {
                        // Kiểm tra nếu StudentID đã tồn tại với một sinh viên khác
                        if (context.Students.Any(s => s.StudentID == txtStudentId.Text && s.StudentID != student.StudentID))
                        {
                            MessageBox.Show("Mã sinh viên đã tồn tại. Vui lòng nhập mã khác.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        // Cập nhật thông tin sinh viên
                        student.FullName = txtFullname.Text;
                        student.AverageScore = double.Parse(txtAverageScore.Text);
                        student.FacultyID = int.Parse(cmbFaculty.SelectedValue.ToString());

                        // Lưu thay đổi vào cơ sở dữ liệu
                        context.SaveChanges();

                        // Tải lại dữ liệu trong DataGridView
                        BindGrid(context.Students.ToList());

                        // Hiển thị thông báo thành công
                        MessageBox.Show("Chỉnh sửa thông tin sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // Hiển thị thông báo nếu không tìm thấy sinh viên
                        MessageBox.Show("Sinh viên không tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và hiển thị thông báo lỗi
                MessageBox.Show($"Đã xảy ra lỗi khi cập nhật dữ liệu: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            btnCalculateTotal_Click(sender, e);
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                // Tạo đối tượng context kết nối với cơ sở dữ liệu
                using (var context = new Model1())
                {
                    // Tìm sinh viên cần xóa dựa trên StudentID
                    var student = context.Students.FirstOrDefault(s => s.StudentID == txtStudentId.Text);

                    if (student != null)
                    {
                        // Xóa sinh viên khỏi cơ sở dữ liệu
                        context.Students.Remove(student);

                        // Lưu thay đổi vào cơ sở dữ liệu
                        context.SaveChanges();

                        // Tải lại dữ liệu vào DataGridView
                        BindGrid(context.Students.ToList());

                        // Hiển thị thông báo thành công
                        MessageBox.Show("Sinh viên đã được xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // Hiển thị thông báo nếu không tìm thấy sinh viên
                        MessageBox.Show("Sinh viên không tìm thấy!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và hiển thị thông báo lỗi
                MessageBox.Show($"Đã xảy ra lỗi khi xóa dữ liệu: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            btnCalculateTotal_Click(sender, e);
        }
        
        private void InitializeUIComponents()
        {
            // Tạo Button "Tính Tổng"
            Button btnCalculateTotal = new Button
            {
                Text = "Tính Tổng",
                Location = new System.Drawing.Point(10, 10), // Vị trí trên Form
                Width = 100
            };
            btnCalculateTotal.Click += btnCalculateTotal_Click; // Liên kết sự kiện Click

            // Tạo TextBox để hiển thị tổng số lượng sinh viên
            TextBox txtTotalStudents = new TextBox
            {
                Name = "txtTotalStudents", // Đặt tên để dễ truy cập
                Location = new System.Drawing.Point(120, 10), // Vị trí trên Form
                Width = 50,
                ReadOnly = true // Không cho phép chỉnh sửa
            };

            // Thêm các control vào Form
            this.Controls.Add(btnCalculateTotal);
            this.Controls.Add(txtTotalStudents);
        }

        private void btnCalculateTotal_Click(object sender, EventArgs e)
        {
            try
            {
                // Tạo đối tượng context kết nối với cơ sở dữ liệu
                using (var context = new Model1())
                {
                    // Đếm tổng số lượng sinh viên trong cơ sở dữ liệu
                    int totalStudents = context.Students.Count();

                    // Tìm TextBox để hiển thị kết quả
                    TextBox txtTotalStudents = this.Controls["txtTotalStudents"] as TextBox;
                    if (txtTotalStudents != null)
                    {
                        // Hiển thị tổng số lượng sinh viên
                        txtTotalStudents.Text = totalStudents.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                MessageBox.Show($"Lỗi khi tính tổng số lượng sinh viên: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        
    }
}