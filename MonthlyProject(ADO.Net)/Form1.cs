using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonthlyProject_ADO.Net_
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        MemoryStream ms;
        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            BlankTextBoxes();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            ConnectionDB a = new ConnectionDB();
            a.conn1("select distinct Id from student");
            SqlDataReader rdr = a.cmd1.ExecuteReader();//select
            while (rdr.Read())//until last data
            {
                comboBox1.Items.Add(rdr[0].ToString());//0=> first field, Id
            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConnectionDB a = new ConnectionDB();
            a.conn1($"select * from student where id='{comboBox1.Text}'");
            SqlDataReader rdr = a.cmd1.ExecuteReader();//select
            while (rdr.Read())
            {
                textBox1.Text = rdr["Id"].ToString();
                textBox2.Text = rdr["Name"].ToString();
                textBox3.Text = rdr["Fee"].ToString();
                dateTimePicker1.Value = DateTime.Parse(rdr["joindate"].ToString());

                pictureBox1.Image = null;
                if (rdr["Photo"] != System.DBNull.Value)
                {
                    Byte[] byteBLOBData = new Byte[0];
                    byteBLOBData = (Byte[])((byte[])rdr["Photo"]);
                    MemoryStream ms = new MemoryStream(byteBLOBData);
                    ms.Write(byteBLOBData, 0, byteBLOBData.Length);
                    ms.Position = 0; //insert this line
                    pictureBox1.Image = Image.FromStream(ms);
                }
                pictureBox2.ImageLocation = AppDomain.CurrentDomain.BaseDirectory + rdr["stringphoto"].ToString();

            }
            a.conn1($"select * from fees where studentid='{comboBox1.Text}' order by slno");
            //SqlDataReader rdr2 = a.cmd1.ExecuteReader();//select

            //int i = 0;
            //while (rdr2.Read())//until last data
            //{
            //    textBox6.Text = rdr2["vno"].ToString();
            //    DataGridViewRow newRow = new DataGridViewRow();
            //    newRow.CreateCells(dataGridView1);
            //    newRow.Cells[0].Value = rdr2["slno"];
            //    newRow.Cells[1].Value = rdr2["headname"];
            //    newRow.Cells[2].Value = rdr2["amount"];
            //    dataGridView1.Rows.Add(newRow);
            //    i++;
            //}

            textBox4.Text = pictureBox1.ImageLocation;
            textBox5.Text = pictureBox2.ImageLocation;

            dataGridView1.Columns.Clear();
            SqlDataReader rdr2 = a.cmd1.ExecuteReader();
            DataTable dt = new DataTable();//disconnected class
            dt.Load(rdr2, LoadOption.Upsert);
            dataGridView1.DataSource = dt;

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "jpeg|*.jpg|bmp|*.bmp|all files|*.*";
            DialogResult res = openFileDialog1.ShowDialog();
            if (res == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
                textBox4.Text = openFileDialog1.FileName;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "jpeg|*.jpg|bmp|*.bmp|all files|*.*";
            DialogResult res = openFileDialog1.ShowDialog();
            if (res == DialogResult.OK)
            {
                pictureBox2.ImageLocation = openFileDialog1.FileName;
                textBox5.Text = openFileDialog1.FileName;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            DataGridViewRow newRow = new DataGridViewRow();
            newRow.CreateCells(dataGridView1);
            newRow.Cells[0].Value = textBox7.Text;
            newRow.Cells[1].Value = textBox8.Text;
            newRow.Cells[2].Value = textBox9.Text;
            dataGridView1.Rows.Add(newRow);
        }
        byte[] conv_photo()
        {
            byte[] photo_aray = { };
            //converting photo to binary data
            if (pictureBox1.Image != null)
            {
                ms = new MemoryStream();
                pictureBox1.Image.Save(ms, ImageFormat.Jpeg);
                photo_aray = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(photo_aray, 0, photo_aray.Length);
            }
            return photo_aray;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            ConnectionStringSettings student;
            student = ConfigurationManager.ConnectionStrings["exam"];
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = student.ConnectionString;
                cn.Open();
                using (SqlTransaction tran = cn.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand cmd = cn.CreateCommand())
                        {
                            cmd.CommandText = $"delete from fees where studentid='{textBox1.Text}'";
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();

                            cmd.CommandText = $"delete from student where id='{textBox1.Text}'";
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();



                            string fn = Path.GetFileName(textBox5.Text);
                            string path = AppDomain.CurrentDomain.BaseDirectory + @"Images\" + fn.ToString();
                            if (!File.Exists(path))
                            {
                                File.Copy(textBox5.Text, path);
                            }
                            string dt = dateTimePicker1.Value.ToShortDateString();

                            cmd.CommandText = $"insert into student values(@id, @name, @fee, @dt, @pic, @picstring)";
                            cmd.Parameters.Add(new SqlParameter("@id", textBox1.Text));
                            cmd.Parameters.Add(new SqlParameter("@name", textBox2.Text));
                            cmd.Parameters.Add(new SqlParameter("@fee", double.Parse(textBox3.Text)));
                            cmd.Parameters.Add(new SqlParameter("@dt", dt));
                            cmd.Parameters.Add(new SqlParameter("@pic", conv_photo()));
                            cmd.Parameters.Add(new SqlParameter("@picstring", "Images\\" + fn.ToString()));
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();

                            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                            {
                                cmd.CommandText = $"insert into fees values('{textBox6.Text}', {int.Parse(dataGridView1.Rows[i].Cells[0].Value.ToString())}, '{dataGridView1.Rows[i].Cells[1].Value.ToString()}', '{dataGridView1.Rows[i].Cells[2].Value.ToString()}', '{textBox1.Text}')";
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();


                            }
                            tran.Commit();

                        }

                        dataGridView1.Rows.Clear();
                    }
                    catch (Exception xcp)
                    {
                        tran.Rollback();
                        MessageBox.Show(xcp.ToString());
                    }
                }
            }

        }
        private void BlankTextBoxes()
        {
            try
            {
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
                textBox5.Text = "";
                textBox6.Text = "";
                textBox7.Text = "";
                textBox8.Text = "";
                textBox9.Text = "";
                dataGridView1.DataSource = null;
                    dataGridView1.Rows.Clear();
                textBox1.Focus();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ConnectionStringSettings student;
            student = ConfigurationManager.ConnectionStrings["exam"];
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = student.ConnectionString;
                cn.Open();
                using (SqlTransaction tran = cn.BeginTransaction())
                {
                    
                        using (SqlCommand cmd = cn.CreateCommand())
                        {
                        try
                        {
                            cmd.CommandText = $"delete from fees where studentid='{textBox1.Text}'";
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();

                            cmd.CommandText = $"delete from student where id='{textBox1.Text}'";
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            tran.Commit();
                            dataGridView1.DataSource = null;
                            dataGridView1.Rows.Clear();
                        }
                        catch (Exception ex)
                        {

                            MessageBox.Show(ex.Message.ToString());
                            tran.Rollback();
                        }
                    }
                    
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {



            dataGridView1.Columns.Add("SlNo", "Sl No");
            dataGridView1.Columns.Add("HeadName", "Head Name");
            dataGridView1.Columns.Add("Amount", "Amount");
        



    }

        private void button6_Click(object sender, EventArgs e)
        {
            Form2 a = new Form2();
            a.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Form3 a = new Form3();
            a.Show();
        }
        public static string GetComBo = "";
        private void button8_Click(object sender, EventArgs e)
        {
            GetComBo = comboBox1.Text;
            Form4 a = new Form4();
            a.Show();

        }
    }
}


