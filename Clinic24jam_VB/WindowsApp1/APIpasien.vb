Imports System.Net.Http
Imports System.Text
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Public Class APIpasien

    Public Async Function GetAreaUserAsync() As Task
        ' Pastikan kolom sudah ditambahkan (jika belum)
        If Form1.dgvDataPasien.Columns.Count = 0 Then
            Form1.dgvDataPasien.Columns.Add("id", "ID") ' Tambah kolom ID
            Form1.dgvDataPasien.Columns.Add("nik", "NIK")
            Form1.dgvDataPasien.Columns.Add("name", "Nama")
            Form1.dgvDataPasien.Columns.Add("gender", "Jenis Kelamin")
            Form1.dgvDataPasien.Columns.Add("date_of_birth", "Tanggal Lahir")
            Form1.dgvDataPasien.Columns.Add("address", "Alamat")
            Form1.dgvDataPasien.Columns.Add("password", "Password")
        End If

        Try
            Dim baseAddress As String = "https://impala-famous-cattle.ngrok-free.app/"  ' Pastikan ada trailing slash
            Using client As New HttpClient()
                client.BaseAddress = New Uri(baseAddress)

                ' Pastikan URL endpoint yang benar adalah "patients"
                Dim response As HttpResponseMessage = Await client.GetAsync("patients")
                response.EnsureSuccessStatusCode()

                Dim responseBody As String = Await response.Content.ReadAsStringAsync()
                ' Log response body for debugging
                Console.WriteLine(responseBody)

                ' Deserialize JSON respons
                Dim userList As List(Of Dictionary(Of String, Object)) = JsonConvert.DeserializeObject(Of List(Of Dictionary(Of String, Object)))(responseBody)

                ' Clear existing rows if any
                Form1.dgvDataPasien.Rows.Clear()

                For Each item As Dictionary(Of String, Object) In userList
                    ' Pastikan key yang diambil sesuai dengan respons dari server
                    Dim id = If(item.ContainsKey("id"), item("id").ToString(), String.Empty)
                    Dim nik = If(item.ContainsKey("nik"), item("nik").ToString(), String.Empty)
                    Dim nama = If(item.ContainsKey("name"), item("name").ToString(), String.Empty)
                    Dim jenisKelamin = If(item.ContainsKey("gender"), item("gender").ToString(), String.Empty)
                    Dim tanggalLahir = If(item.ContainsKey("date_of_birth"), item("date_of_birth").ToString(), String.Empty)
                    Dim alamat = If(item.ContainsKey("address"), item("address").ToString(), String.Empty)
                    Dim password = If(item.ContainsKey("password"), item("password").ToString(), String.Empty)

                    ' Log each item for debugging
                    Console.WriteLine($"ID: {id}, NIK: {nik}, Nama: {nama}, Jenis Kelamin: {jenisKelamin}, Tanggal Lahir: {tanggalLahir}, Alamat: {alamat}, Password: {password}")

                    ' Tambah data ke DataGridView
                    Form1.dgvDataPasien.Rows.Add(id, nik, nama, jenisKelamin, tanggalLahir, alamat, "*****")
                Next

                ' Sembunyikan kolom ID dari tampilan pengguna
                Form1.dgvDataPasien.Columns("id").Visible = False
                Form1.dgvDataPasien.Columns("id").ReadOnly = True ' Jadikan kolom ID tidak dapat diubah

                ' Refresh DataGridView
                Form1.dgvDataPasien.Refresh()

            End Using

        Catch ex As Exception
            MessageBox.Show(ex.Message, "Notifikasi Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Try
    End Function
    Public Async Function PostAreaUserAsync() As Task(Of String)
        Try
            Dim baseAddress As String = "https://impala-famous-cattle.ngrok-free.app/"  ' Pastikan ada trailing slash
            Using client As New HttpClient()
                client.BaseAddress = New Uri(baseAddress)

                ' Membuat objek data untuk dikirimkan sebagai JSON
                Dim userData As New With {
                .nik = Form1.txtNik.Text,
                .name = Form1.txtNama.Text,
                .gender = Form1.cmbGender.Text,
                .date_of_birth = Convert.ToDateTime(Form1.dtpTtl.Value).ToString("yyyy-MM-dd"),
                .address = Form1.txtAlamat.Text,
                .password = Form1.txtPassword.Text  ' Menambah field password
            }

                Dim json As String = JsonConvert.SerializeObject(userData)
                Dim content As New StringContent(json, Encoding.UTF8, "application/json")

                ' URL endpoint adalah "patients"
                Dim response As HttpResponseMessage = Await client.PostAsync("patients", content)
                response.EnsureSuccessStatusCode()

                ' Mendapatkan ID dari respons JSON
                Dim responseContent As String = Await response.Content.ReadAsStringAsync()
                Dim responseData As JObject = JObject.Parse(responseContent)
                Dim id As String = responseData("id").ToString()

                MessageBox.Show("Pendaftaran Berhasil", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)

                Return id ' Mengembalikan ID dari data yang baru saja disimpan
            End Using

        Catch ex As Exception
            MessageBox.Show(ex.Message, "Notifikasi Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Return "" ' Mengembalikan string kosong jika terjadi kesalahan
        End Try
    End Function



    Public Async Function UpdateAreaUserAsync(id As String) As Task
        Try
            Dim baseAddress As String = "https://impala-famous-cattle.ngrok-free.app/"  ' Pastikan ada trailing slash
            Using client As New HttpClient()
                client.BaseAddress = New Uri(baseAddress)
                Dim userData As New With {
                    .nik = Form1.txtNik.Text,
                    .name = Form1.txtNama.Text,
                    .gender = Form1.cmbGender.Text,
                    .date_of_birth = Convert.ToDateTime(Form1.dtpTtl.Value).ToString("yyyy-MM-dd"),
                    .address = Form1.txtAlamat.Text,
                    .password = Form1.txtPassword.Text
                }

                Dim json As String = JsonConvert.SerializeObject(userData)
                Dim content As New StringContent(json, Encoding.UTF8, "application/json")

                ' URL endpoint adalah "patients/{id}" untuk update
                Dim response As HttpResponseMessage = Await client.PutAsync($"patients/{id}", content)
                response.EnsureSuccessStatusCode()

                MessageBox.Show("Update Berhasil", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End Using

        Catch ex As HttpRequestException
            ' Ambil pesan error dari response body jika ada
            Dim errorMsg As String = ex.Message
            MessageBox.Show($"Request Error: {errorMsg}", "Notifikasi Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Notifikasi Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Try
    End Function

    Public Async Function DeleteUserAsync(id As String) As Task
        Try
            Dim baseAddress As String = "https://impala-famous-cattle.ngrok-free.app/"
            Using client As New HttpClient()
                client.BaseAddress = New Uri(baseAddress)

                Dim response As HttpResponseMessage = Await client.DeleteAsync($"patients/{id}")
                response.EnsureSuccessStatusCode()

                MessageBox.Show("Penghapusan Berhasil", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End Using
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Notifikasi Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Try
    End Function

End Class
