Public Class Form1
    Private Sub RadioButton2_CheckedChanged(sender As Object, e As EventArgs)

    End Sub

    Private Sub Label10_Click(sender As Object, e As EventArgs) Handles Label10.Click

    End Sub

    Private Sub Panel2_Paint(sender As Object, e As PaintEventArgs) Handles Panel2.Paint

    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim output_class As New APIpasien
        Call output_class.GetAreaUserAsync()
        txtID.Enabled = False
    End Sub

    Private Async Sub btnSimpan_Click(sender As Object, e As EventArgs) Handles btnSimpan.Click
        Try
            Dim output_class As New APIpasien()
            Dim id As String = Await output_class.PostAreaUserAsync()

            If Not String.IsNullOrEmpty(id) Then
                ' Menambahkan data pasien baru ke DataGridView dengan ID dari server
                Dim newRow As New DataGridViewRow()
                newRow.CreateCells(dgvDataPasien)
                newRow.Cells(0).Value = id ' Menggunakan ID dari server
                newRow.Cells(1).Value = txtNik.Text
                newRow.Cells(2).Value = txtNama.Text
                newRow.Cells(3).Value = cmbGender.Text
                newRow.Cells(4).Value = Convert.ToDateTime(dtpTtl.Value).ToString("yyyy-MM-dd")
                newRow.Cells(5).Value = txtAlamat.Text
                newRow.Cells(6).Value = "*****" ' Menyembunyikan password dengan bintang

                dgvDataPasien.Rows.Add(newRow)

                ' Refresh DataGridView untuk memastikan data baru ditampilkan
                dgvDataPasien.Refresh()

                ' Membersihkan input setelah simpan
                ClearInputFields()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Notifikasi Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Try
    End Sub

    Private Async Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        Try
            Dim id As String = txtID.Text ' Ambil ID dari txtID
            Dim output As New APIpasien

            ' Konfirmasi sebelum melakukan update
            Dim confirmResult As DialogResult = MessageBox.Show("Anda yakin ingin menyimpan perubahan ini?", "Konfirmasi Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

            If confirmResult = DialogResult.Yes Then
                ' Panggil fungsi UpdateAreaUserAsync dengan memberikan id
                Await output.UpdateAreaUserAsync(id)

                ' Update DataGridView setelah berhasil memperbarui data
                Dim selectedRow As DataGridViewRow = dgvDataPasien.SelectedRows(0)
                selectedRow.Cells("nik").Value = txtNik.Text
                selectedRow.Cells("name").Value = txtNama.Text
                selectedRow.Cells("gender").Value = cmbGender.Text
                selectedRow.Cells("date_of_birth").Value = Convert.ToDateTime(dtpTtl.Value).ToString("yyyy-MM-dd")
                selectedRow.Cells("address").Value = txtAlamat.Text
                selectedRow.Cells("password").Value = New String("*"c, txtPassword.Text.Length) ' Mengganti dengan karakter bintang

                ' Kosongkan input fields setelah update berhasil
                ClearInputFields()

                ' Kosongkan juga field ID setelah update
                txtID.Text = ""
            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message, "Notifikasi Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Try
    End Sub
    Private Sub Label6_Click(sender As Object, e As EventArgs) Handles Label6.Click

    End Sub

    Private Sub dgvDataPasien_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvDataPasien.CellContentClick

    End Sub

    Private Sub dgvDataPasien_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvDataPasien.CellDoubleClick
        Try
            If e.RowIndex >= 0 AndAlso e.RowIndex < dgvDataPasien.Rows.Count Then
                Dim idValue As Object = dgvDataPasien.Rows(e.RowIndex).Cells(0).Value
                Dim nikValue As Object = dgvDataPasien.Rows(e.RowIndex).Cells(1).Value
                Dim namaValue As Object = dgvDataPasien.Rows(e.RowIndex).Cells(2).Value
                Dim genderValue As Object = dgvDataPasien.Rows(e.RowIndex).Cells(3).Value
                Dim birthDateValue As Object = dgvDataPasien.Rows(e.RowIndex).Cells(4).Value
                Dim addressValue As Object = dgvDataPasien.Rows(e.RowIndex).Cells(5).Value
                Dim passwordValue As Object = dgvDataPasien.Rows(e.RowIndex).Cells(6).Value

                If idValue IsNot Nothing AndAlso nikValue IsNot Nothing AndAlso namaValue IsNot Nothing AndAlso
           genderValue IsNot Nothing AndAlso birthDateValue IsNot Nothing AndAlso
           addressValue IsNot Nothing AndAlso passwordValue IsNot Nothing Then

                    ' Tampilkan data pada field jika baris yang dipilih valid
                    txtID.Text = idValue.ToString()
                    txtNik.Text = nikValue.ToString()
                    txtNama.Text = namaValue.ToString()
                    cmbGender.Text = genderValue.ToString()
                    dtpTtl.Value = Convert.ToDateTime(birthDateValue)
                    txtAlamat.Text = addressValue.ToString()
                    txtPassword.UseSystemPasswordChar = False ' Tampilkan password
                    txtPassword.Text = passwordValue.ToString()
                    txtID.Enabled = False
                    txtNik.Focus()
                Else
                    ' Kosongkan semua field jika ada nilai yang kosong
                    ClearInputFields()
                End If
            Else
                MessageBox.Show("Pilih baris yang valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Catch ex As Exception
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ClearInputFields()
        txtID.Text = ""
        txtNik.Text = ""
        txtNama.Text = ""
        cmbGender.SelectedIndex = -1
        dtpTtl.Value = DateTime.Now
        txtAlamat.Text = ""
        txtPassword.UseSystemPasswordChar = True ' Kembalikan password field ke bentuk bintang
        txtPassword.Text = ""
        txtID.Enabled = True
    End Sub

    Private Async Sub btnHapus_Click(sender As Object, e As EventArgs) Handles btnHapus.Click
        Try
            ' Ambil ID user yang akan dihapus dari inputan atau sumber data lainnya
            Dim userId As String = txtID.Text ' Misalnya dari TextBox txtID
            Dim output As New APIpasien
            txtID.Enabled = False

            ' Konfirmasi sebelum menghapus
            Dim confirmResult As DialogResult = MessageBox.Show("Anda yakin ingin menghapus data ini?", "Konfirmasi Penghapusan", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

            If confirmResult = DialogResult.Yes Then
                ' Panggil fungsi DeleteUserAsync dengan memberikan userId sebagai parameter
                Await output.DeleteUserAsync(userId)

                ' Kosongkan input fields setelah penghapusan berhasil
                ClearInputFields()

                ' Bersihkan isi DataGridView
                dgvDataPasien.Rows.Clear()

                ' Muat ulang data setelah penghapusan
                Await output.GetAreaUserAsync()
            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message, "Notifikasi Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Try
    End Sub


    Private Sub txtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged
        Dim searchText As String = txtSearch.Text.Trim().ToLower()

        ' Menggunakan LINQ untuk filter data di DataGridView
        For Each row As DataGridViewRow In dgvDataPasien.Rows
            ' Pengecekan untuk menghindari InvalidOperationException
            If Not row.IsNewRow Then
                Dim nikCellValue As String = If(row.Cells("nik").Value, String.Empty).ToString().ToLower()
                Dim nameCellValue As String = If(row.Cells("name").Value, String.Empty).ToString().ToLower()

                ' Cek apakah NIK atau Nama mengandung teks pencarian
                If nikCellValue.Contains(searchText) OrElse nameCellValue.Contains(searchText) Then
                    row.Visible = True
                Else
                    row.Visible = False
                End If
            End If
        Next
    End Sub
    Private Sub txtBatal_Click(sender As Object, e As EventArgs) Handles txtBatal.Click
        ClearInputFields()
        txtID.Enabled = False
    End Sub

    Private Sub btnCari_Click(sender As Object, e As EventArgs) Handles btnCari.Click
        btnCari.Enabled = False
        btnCari.Enabled = True
    End Sub
End Class

