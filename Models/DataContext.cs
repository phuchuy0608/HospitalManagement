using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using HospitalManagement_Entities.Models.ViewModel;
using HospitalManagement.Models;



namespace HospitalManagement.Models
{
    public partial class DataContext : IdentityDbContext<NhanVienYte>
    {
        public DataContext(DbContextOptions<DataContext> options)
           : base(options)
        {
        }
        public virtual DbSet<Benh> Benh {get; set;}
        public virtual DbSet<BenhNhan> BenhNhan {get; set;}
        
        public virtual DbSet<CTTrieuChung> CTTrieuChung { get; set;}
        public virtual DbSet<ChiTietDV> ChiTietDV { get; set; }
        public virtual DbSet<ChiTietSinhHieu> ChiTietSinhHieu { get; set; }
        public virtual DbSet<ChiTietToaThuoc> ChiTietToaThuoc { get; set; }
        public virtual DbSet<ChuyenKhoa> ChuyenKhoa { get; set; }
        public virtual DbSet<DichVu> DichVu { get; set; }
        public virtual DbSet<HoaDon> HoaDon { get; set; }
        public virtual DbSet<HoaDonThuoc> HoaDonThuoc { get; set; }
        public virtual DbSet<NguoiDung> NguoiDung { get; set; }
        public virtual DbSet<NhanVienYte> NhanVienYte { get; set; }
        public virtual DbSet<PhieuDatLich> PhieuDatLich { get; set; }
        public virtual DbSet<PhieuKham> PhieuKham { get; set; }
        public virtual DbSet<STTPhieuKham> STTPhieuKham { get; set; }
        public virtual DbSet<STTTOATHUOC> STTTOATHUOC { get; set; }
        public virtual DbSet<Thuoc> Thuoc { get; set; }
        public virtual DbSet<TinTuc> TinTuc { get; set; }
        public virtual DbSet<ToaThuoc> ToaThuoc { get; set; }
        public virtual DbSet<TrieuChung> TrieuChung { get; set; }
        public virtual DbSet<TheLoai> TheLoai { get; set; }
        public virtual DbSet<ChiTietBenh> ChiTietBenh { get; set; }
        public virtual DbSet<ThongKeDichVuViewModel> ThongKeViewModel { get; set; }
        public virtual DbSet<ListResponse> ListResponses { get; set; }
        public virtual DbSet<ResponseChanDoan> ResponseChanDoans { get; set; }
        public virtual DbSet<ResponseHoaDon> ResponseHoaDons { get; set; }
        public virtual DbSet<ScalarInt> ScalarInt { get; set; }
        public virtual DbSet<ThongKeBenhViewModel> ThongKeBenhViewModel { get; set; }
        public virtual DbSet<ThongKeSoLuongThuoc> ThongKeSLThuocViewModel { get; set; }
        public virtual DbSet<ThongKeLuotKhamViewModel> ThongKeLuotKhamViewModel { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

            }
        }
 protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ThongKeDichVuViewModel>(entiy => entiy.HasNoKey().ToView(null));
            modelBuilder.Entity<ListResponse>(entiy => entiy.HasNoKey().ToView(null));
            modelBuilder.Entity<ResponseChanDoan>(entiy => entiy.HasNoKey().ToView(null));
            modelBuilder.Entity<ResponseHoaDon>(entiy => entiy.HasNoKey().ToView(null));
            modelBuilder.Entity<ScalarInt>(entiy => entiy.HasNoKey().ToView(null));

            modelBuilder.Entity<ThongKeBenhViewModel>(entiy => entiy.HasNoKey().ToView(null));
            modelBuilder.Entity<ThongKeSoLuongThuoc>(entiy => entiy.HasNoKey().ToView(null));
            modelBuilder.Entity<ThongKeLuotKhamViewModel>(entiy => entiy.HasNoKey().ToView(null));

            base.OnModelCreating(modelBuilder);
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");


            modelBuilder.Entity<TheLoai>(entity =>
            {
                entity.HasKey(e => e.MaTL)
                    .HasName("PK__TheLoai__272500715188CB73");

                entity.Property(e => e.MaTL).ValueGeneratedNever();

                entity.Property(e => e.TenTL)
                    .IsRequired()
                    .HasMaxLength(100);
            });




            modelBuilder.Entity<TinTuc>(entity =>
            {
                entity.HasKey(e => e.MaBaiViet)
                    .HasName("PK__TinTuc__AEDD56476D752A3D");

                entity.HasIndex(e => e.MaNguoiViet, "IX_TinTuc_MaNguoiViet");

                entity.Property(e => e.MaBaiViet).ValueGeneratedNever();

                entity.Property(e => e.NoiDung).IsRequired();

                entity.Property(e => e.TieuDe)
                    .IsRequired()
                    .HasMaxLength(250);
                entity.Property(e => e.ThoiGian)
                   
                   .HasColumnType("DateTime");

                entity.HasOne(d => d.MaNguoiVietNavigation)
                    .WithMany(p => p.TinTuc)
                    .HasForeignKey(d => d.MaNguoiViet)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TinTuc__MaNguoiV__1273C1CD");

                entity.HasOne(d => d.MaTLNavigation)
                    .WithMany(p => p.TinTuc)
                    .HasForeignKey(d => d.MaTL)
                    .HasConstraintName("FK__TinTuc__MaTL__71D1E811");
            });


            modelBuilder.Entity<Benh>(entity =>
            {
                entity.HasKey(e => e.MaBenh)
                    .HasName("PK__Benh__DB7E2D498DB80DEE");

                entity.Property(e => e.MaBenh).ValueGeneratedNever();

                entity.Property(e => e.TenBenh)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.MaCKNavigation)
                    .WithMany(p => p.Benh)
                    .HasForeignKey(d => d.MaCK)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Benh__MaCK__1CF15040");
            });

            modelBuilder.Entity<BenhNhan>(entity =>
            {
                entity.HasKey(e => e.MaBN)
                    .HasName("PK__BenhNhan__272475AD90105519");

                entity.Property(e => e.MaBN).ValueGeneratedNever();

                entity.Property(e => e.DiaChi)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.HoTen)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.NgaySinh).HasColumnType("datetime");

                entity.Property(e => e.SDT)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);
            });
            modelBuilder.Entity<ChiTietBenh>(entity =>
            {
                entity.HasKey(e => new { e.MaPK, e.MaBenh })
                    .HasName("PK__ChiTietBenh__339EF89FCA764F6C");

               
                entity.HasOne(d => d.MaPKNavigation)
                    .WithMany(p => p.ChiTietBenh)
                    .HasForeignKey(d => d.MaPK)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ChiTietBenh__MaPK__3A81B327");

                entity.HasOne(d => d.MaBenhNavigation)
                    .WithMany(p => p.ChiTietBenh)
                    .HasForeignKey(d => d.MaBenh)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ChiTietBenh__MaBenh__3B75D760");
            });

            modelBuilder.Entity<CTTrieuChung>(entity =>
            {
                entity.HasKey(e => new { e.MaBenh, e.MaTrieuChung })
                    .HasName("PK__CTTrieuC__E45FC2F731FF98AB");

            

                entity.HasOne(d => d.MaBenhNavigation)
                    .WithMany(p => p.CTTrieuChung)
                    .HasForeignKey(d => d.MaBenh)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CTTrieuCh__MaBen__19DFD96B");

                entity.HasOne(d => d.MaTrieuChungNavigation)
                    .WithMany(p => p.CTTrieuChung)
                    .HasForeignKey(d => d.MaTrieuChung)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CTTrieuCh__MaTri__1AD3FDA4");
            });

            modelBuilder.Entity<ChiTietDV>(entity =>
            {
                entity.HasKey(e => new { e.MaHD, e.MaDV })
                    .HasName("PK__ChiTietD__4557FE8526B532A5");

                entity.Property(e => e.MaHD)
                    .HasMaxLength(20)
                    .IsUnicode(false);
                entity.Property(e => e.DonGiaDV).HasColumnType("money");
                entity.HasOne(d => d.MaDVNavigation)
                    .WithMany(p => p.ChiTietDV)
                    .HasForeignKey(d => d.MaDV)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ChiTietDV__MaDV__40058253");

                entity.HasOne(d => d.MaHDNavigation)
                    .WithMany(p => p.ChiTietDV)
                    .HasForeignKey(d => d.MaHD)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ChiTietDV__MaHD__3F115E1A");
            });


            modelBuilder.Entity<ChiTietSinhHieu>(entity =>
            {
                entity.HasKey(e => e.MaSinhHieu)
                    .HasName("PK__ChiTietS__F33E637FDFBD41BE");

                entity.Property(e => e.MaSinhHieu).ValueGeneratedNever();

                entity.Property(e => e.TenSH)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ThongTinChiTiet).IsRequired();

                entity.HasOne(d => d.MaPKNavigation)
                    .WithMany(p => p.ChiTietSinhHieu)
                    .HasForeignKey(d => d.MaPK)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ChiTietSin__MaPK__35BCFE0A");
            });

            modelBuilder.Entity<ChiTietToaThuoc>(entity =>
            {
                entity.HasKey(e => new { e.MaPK, e.MaThuoc })
                    .HasName("PK__ChiTietT__339EF89FCA764F6C");

                entity.Property(e => e.GhiChu);
                entity.Property(e => e.DonGiaThuoc).HasColumnType("money");
                entity.HasOne(d => d.MaPKNavigation)
                    .WithMany(p => p.ChiTietToaThuoc)
                    .HasForeignKey(d => d.MaPK)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ChiTietToa__MaPK__3A81B327");

                entity.HasOne(d => d.MaThuocNavigation)
                    .WithMany(p => p.ChiTietToaThuoc)
                    .HasForeignKey(d => d.MaThuoc)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ChiTietTo__MaThu__3B75D760");
            });

            modelBuilder.Entity<ChuyenKhoa>(entity =>
            {
                entity.HasKey(e => e.MaCK)
                    .HasName("PK__ChuyenKh__27258E0D1F0FE375");

                entity.Property(e => e.MaCK).ValueGeneratedNever();

                entity.Property(e => e.TenCK)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<DichVu>(entity =>
            {
                entity.HasKey(e => e.MaDV)
                    .HasName("PK__DichVu__2725865790C1C308");

                entity.Property(e => e.MaDV).ValueGeneratedNever();

                entity.Property(e => e.DonGia).HasColumnType("money");

                entity.Property(e => e.TenDV)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.DonGia).IsRequired().HasColumnType("money");
            });
            modelBuilder.Entity<HoaDon>(entity =>
            {
                entity.HasKey(e => e.MaHoaDon)
                    .HasName("PK__HoaDon__835ED13BE8E6487A");

                entity.HasIndex(e => e.MaNV, "IX_HoaDon_MaNV");

                entity.HasIndex(e => e.MaPK, "IX_HoaDon_MaPK");

                entity.Property(e => e.MaHoaDon)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.MaNV)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NgayHD).HasColumnType("datetime");

                entity.Property(e => e.TongTien).HasColumnType("money");

                entity.HasOne(d => d.MaNVNavigation)
                    .WithMany(p => p.HoaDon)
                    .HasForeignKey(d => d.MaNV)
                    .HasConstraintName("FK__HoaDon__MaNV__35BCFE0A");

                entity.HasOne(d => d.MaPKNavigation)
                    .WithMany(p => p.HoaDon)
                    .HasForeignKey(d => d.MaPK)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__HoaDon__MaPK__34C8D9D1");
            });

            modelBuilder.Entity<HoaDonThuoc>(entity =>
            {
                entity.HasKey(e => e.MaPK)
                    .HasName("PK__HoaDonTh__2725E7FD6887BBCA");

                entity.HasIndex(e => e.MaHD, "UQ__HoaDonTh__2725A6E187D21950")
                    .IsUnique();

                entity.Property(e => e.MaPK).ValueGeneratedNever();

                entity.Property(e => e.MaHD)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.NgayHD).HasColumnType("datetime");

                entity.Property(e => e.TongTien).HasColumnType("money");

                entity.HasOne(d => d.MaNVNavigation)
                    .WithMany(p => p.HoaDonThuoc)
                    .HasForeignKey(d => d.MaNV)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__HoaDonThuo__MaNV__440B1D61");

                entity.HasOne(d => d.MaPKNavigation)
                    .WithOne(p => p.HoaDonThuoc)
                    .HasForeignKey<HoaDonThuoc>(d => d.MaPK)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__HoaDonThuo__MaPK__4316F928");
            });


            modelBuilder.Entity<NguoiDung>(entity =>
            {
                entity.HasKey(e => e.MaNguoiDung)
                    .HasName("PK__NguoiDun__C539D7624ABCA545");

                entity.Property(e => e.MaNguoiDung).ValueGeneratedNever();

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.HinhAnh).HasMaxLength(250);

                entity.Property(e => e.HoTen)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.MatKhau)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.SDT)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<NhanVienYte>(entity =>
            {
                modelBuilder.Entity<NhanVienYte>().ToTable("NhanVienYTe");
                entity.HasKey(e => e.Id)
                    .HasName("PK__NhanVien__2725D70A99F2EA0C");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Hinh).HasMaxLength(250);

                entity.Property(e => e.HoTen)
                    .IsRequired()
                    .HasMaxLength(50);

             

               

                entity.HasOne(d => d.ChuyenKhoaNavigation)
                    .WithMany(p => p.NhanVienYte)
                    .HasForeignKey(d => d.ChuyenKhoa)
                    .HasConstraintName("FK__NhanVienY__Chuye__20C1E124");
            });

            modelBuilder.Entity<PhieuDatLich>(entity =>
            {
                entity.HasKey(e => e.MaPhieu)
                    .HasName("PK__PhieuDat__2660BFE037FE92E3");

                entity.Property(e => e.MaPhieu).ValueGeneratedNever();

                entity.Property(e => e.Email)

                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NgayKham).HasColumnType("datetime");

                entity.Property(e => e.NgaySinh).HasColumnType("datetime");

                entity.Property(e => e.SDT)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.TenBN)
                    .IsRequired()
                    .HasMaxLength(50);
            });


            modelBuilder.Entity<PhieuKham>(entity =>
            {
                entity.HasKey(e => e.MaPK)
                    .HasName("PK__PhieuKha__2725E7FDD3F81957");

                entity.HasIndex(e => e.MaBN, "IX_PhieuKham_MaBN");

                entity.HasIndex(e => e.MaBS, "IX_PhieuKham_MaBS");

                entity.Property(e => e.MaPK).ValueGeneratedNever();

                entity.Property(e => e.HuyetAp)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.MaBS)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Mach).HasMaxLength(50);

                entity.Property(e => e.NgayKham).HasColumnType("datetime");

                entity.Property(e => e.NgayTaiKham).HasColumnType("datetime");

                entity.Property(e => e.NhietDo)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.TrangThai).HasDefaultValueSql("(CONVERT([tinyint],(0)))");

                entity.HasOne(d => d.MaBNNavigation)
                    .WithMany(p => p.PhieuKham)
                    .HasForeignKey(d => d.MaBN)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PhieuKham__MaBN__2B3F6F97");

                entity.HasOne(d => d.MaBSNavigation)
                    .WithMany(p => p.PhieuKham)
                    .HasForeignKey(d => d.MaBS)
                    .HasConstraintName("FK__PhieuKham__MaBS__2A4B4B5E");
              
            });



            modelBuilder.Entity<STTPhieuKham>(entity =>
            {
                entity.HasKey(e => e.MaPhieuKham)
                    .HasName("PK__STTPhieu__FACA55DF6DAFE0DA");

                entity.Property(e => e.MaPhieuKham).ValueGeneratedNever();

                entity.Property(e => e.MaUuTien)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.HasOne(d => d.MaPhieuKhamNavigation)
                    .WithOne(p => p.STTPhieuKham)
                    .HasForeignKey<STTPhieuKham>(d => d.MaPhieuKham)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__STTPhieuK__MaPhi__2B3F6F97");
            });

            modelBuilder.Entity<Thuoc>(entity =>
            {
                entity.HasKey(e => e.MaThuoc)
                    .HasName("PK__Thuoc__4BB1F620DA17E5F3");

                entity.Property(e => e.MaThuoc).ValueGeneratedNever();

                entity.Property(e => e.DonGia).HasColumnType("money");
                
                entity.Property(e => e.HinhAnh).HasMaxLength(250);

                entity.Property(e => e.TenThuoc)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.ThongTin).IsRequired();

                entity.Property(e => e.Vitri)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);
                entity.Property(e => e.DonGia).IsRequired().HasColumnType("money");
            });
            modelBuilder.Entity<STTTOATHUOC>(entity =>
            {
                entity.HasKey(e => e.MaPK)
                    .HasName("PK__STTTOATH__2725E7FDFEC58D61");

                entity.Property(e => e.MaPK).ValueGeneratedNever();

                entity.Property(e => e.UuTien)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.HasOne(d => d.MaPKNavigation)
                    .WithOne(p => p.STTTOATHUOC)
                    .HasForeignKey<STTTOATHUOC>(d => d.MaPK)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__STTTOATHUO__MaPK__35BCFE0A");
            });
            modelBuilder.Entity<TinTuc>(entity =>
            {
                entity.HasKey(e => e.MaBaiViet)
                    .HasName("PK__TinTuc__AEDD5647A9FE161F");

                entity.Property(e => e.MaBaiViet).ValueGeneratedNever();

                entity.Property(e => e.NoiDung).IsRequired();

                entity.Property(e => e.TieuDe)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.HasOne(d => d.MaNguoiVietNavigation)
                    .WithMany(p => p.TinTuc)
                    .HasForeignKey(d => d.MaNguoiViet)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TinTuc__MaNguoiV__1273C1CD");
            });

            modelBuilder.Entity<ToaThuoc>(entity =>
            {
                entity.HasKey(e => e.MaPhieuKham)
                    .HasName("PK__ToaThuoc__FACA55DF1E8609CC");

                entity.Property(e => e.MaPhieuKham).ValueGeneratedNever();

                entity.HasOne(d => d.MaPhieuKhamNavigation)
                    .WithOne(p => p.ToaThuoc)
                    .HasForeignKey<ToaThuoc>(d => d.MaPhieuKham)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ToaThuoc__MaPhie__38996AB5");
            });
            modelBuilder.Entity<TrieuChung>(entity =>
            {
                entity.HasKey(e => e.MatrieuChung)
                    .HasName("PK__TrieuChu__18521B702BAAC1B3");

                entity.HasIndex(e => e.TenTrieuChung, "UQ__TrieuChu__38C0D5667814C499")
                    .IsUnique();

                entity.Property(e => e.MatrieuChung).ValueGeneratedNever();

                entity.Property(e => e.TenTrieuChung).HasMaxLength(100);

              
            });



            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
