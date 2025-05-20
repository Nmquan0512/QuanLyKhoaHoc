using System;
using System.Collections.Generic;
using System.IO;

enum CourseLevel
{
    Beginner,
    Intermediate,
    Advanced
}

abstract class User : Person
{
    public User(string fullName, string email) : base(fullName, email) { }

    public abstract void Login();
    public abstract void Logout();
}

interface ICanLearn
{
    void RegisterCourse(Course course);
    void TakeExam(string courseName, double score);
}

class Person
{
    public string FullName { get; set; }
    public string Email { get; set; }

    public Person() { }

    public Person(string fullName, string email)
    {
        FullName = fullName;
        Email = email;
    }

    public virtual void DisplayInfo()
    {
        Console.WriteLine($"Ho ten: {FullName}, Email: {Email}");
    }
}

class Course
{
    public string Name { get; private set; }
    public CourseLevel Level { get; private set; }

    public Course(string name, CourseLevel level)
    {
        Name = name;
        Level = level;
    }

    public override string ToString()
    {
        return $"{Name} ({Level})";
    }
}

class Enrollment
{
    public Course Course { get; private set; }
    public double Score { get; set; }

    public Enrollment(Course course)
    {
        Course = course;
        Score = -1;
    }

    public string GetLevel()
    {
        if (Score < 0) return "Chua co diem";
        if (Score >= 85) return "Gioi";
        if (Score >= 70) return "Kha";
        if (Score >= 50) return "Trung binh";
        return "Yeu";
    }

    public override string ToString()
    {
        return $"{Course.Name} - Level: {Course.Level} - Diem: {(Score < 0 ? "Chua nhap" : Score.ToString())} - Xep loai: {GetLevel()}";
    }
}

class Student : User, ICanLearn
{
    private List<Enrollment> enrollments = new List<Enrollment>();

    public Student(string fullName, string email) : base(fullName, email) { }

    public override void DisplayInfo()
    {
        Console.WriteLine($"Ho ten: {FullName}, Email: {Email}, So khoa dang ky: {enrollments.Count}");
        foreach (var e in enrollments)
        {
            Console.WriteLine("  - " + e.ToString());
        }
    }

    public override void Login()
    {
        Console.WriteLine($"{FullName} da dang nhap.");
    }

    public override void Logout()
    {
        Console.WriteLine($"{FullName} da dang xuat.");
    }

    public void RegisterCourse(Course course)
    {
        if (enrollments.Exists(e => e.Course.Name == course.Name))
        {
            Console.WriteLine("Ban da dang ky khoa hoc nay.");
            return;
        }
        enrollments.Add(new Enrollment(course));
        Console.WriteLine($"Dang ky khoa hoc {course.Name} thanh cong.");
    }

    public void TakeExam(string courseName, double score)
    {
        var enrollment = enrollments.Find(e => e.Course.Name == courseName);
        if (enrollment == null)
        {
            Console.WriteLine("Ban chua dang ky khoa hoc nay.");
            return;
        }
        enrollment.Score = score;
        Console.WriteLine($"Nhap diem {score} cho khoa hoc {courseName} thanh cong.");
    }

    public List<Enrollment> GetEnrollments()
    {
        return enrollments;
    }
}

class Program
{
    static List<Student> students = new List<Student>();
    static List<Course> courses = new List<Course>();

    static void Main(string[] args)
    {
        courses.Add(new Course("C# Co ban", CourseLevel.Beginner));
        courses.Add(new Course("Lap trinh web", CourseLevel.Intermediate));
        courses.Add(new Course("Lap trinh nang cao", CourseLevel.Advanced));

        bool running = true;

        while (running)
        {
            Console.WriteLine("\n--- MENU CHINH ---");
            Console.WriteLine("1. Them hoc vien");
            Console.WriteLine("2. Dang ky khoa hoc");
            Console.WriteLine("3. Nhap diem");
            Console.WriteLine("4. Hien thi danh sach hoc vien");
            Console.WriteLine("5. Ghi du lieu ra file");
            Console.WriteLine("6. Doc du lieu tu file");
            Console.WriteLine("7. Thoat");
            Console.Write("Chon chuc nang: ");

            string choice = Console.ReadLine();
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    ThemHocVien();
                    break;
                case "2":
                    DangKyKhoaHoc();
                    break;
                case "3":
                    NhapDiem();
                    break;
                case "4":
                    HienThiDanhSach();
                    break;
                case "5":
                    GhiDuLieuRaFile();
                    break;
                case "6":
                    DocDuLieuTuFile();
                    break;
                case "7":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Lua chon khong hop le. Vui long chon lai.");
                    break;
            }
        }
    }

    static void ThemHocVien()
    {
        try
        {
            Console.Write("Nhap ho ten hoc vien: ");
            string name = Console.ReadLine();
            Console.Write("Nhap email hoc vien: ");
            string email = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
            {
                Console.WriteLine("Ho ten va email khong duoc de trong.");
                return;
            }

            students.Add(new Student(name, email));
            Console.WriteLine("Them hoc vien thanh cong.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Loi khi them hoc vien: " + ex.Message);
        }
    }

    static Student TimHocVien()
    {
        Console.Write("Nhap email hoc vien: ");
        string email = Console.ReadLine();
        var student = students.Find(s => s.Email.ToLower() == email.ToLower());
        if (student == null)
        {
            Console.WriteLine("Khong tim thay hoc vien voi email tren.");
        }
        return student;
    }

    static Course TimKhoaHoc()
    {
        Console.WriteLine("Danh sach khoa hoc:");
        for (int i = 0; i < courses.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {courses[i]}");
        }
        Console.Write("Chon khoa hoc (nhap so): ");
        if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > courses.Count)
        {
            Console.WriteLine("Lua chon khoa hoc khong hop le.");
            return null;
        }
        return courses[index - 1];
    }

    static void DangKyKhoaHoc()
    {
        try
        {
            var student = TimHocVien();
            if (student == null) return;

            var course = TimKhoaHoc();
            if (course == null) return;

            student.RegisterCourse(course);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Loi khi dang ky khoa hoc: " + ex.Message);
        }
    }

    static void NhapDiem()
    {
        try
        {
            var student = TimHocVien();
            if (student == null) return;

            var enrollments = student.GetEnrollments();
            if (enrollments.Count == 0)
            {
                Console.WriteLine("Hoc vien chua dang ky khoa hoc nao.");
                return;
            }

            Console.WriteLine("Danh sach khoa hoc da dang ky:");
            for (int i = 0; i < enrollments.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {enrollments[i].Course.Name}");
            }
            Console.Write("Chon khoa hoc de nhap diem (nhap so): ");
            if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > enrollments.Count)
            {
                Console.WriteLine("Lua chon khong hop le.");
                return;
            }

            Console.Write("Nhap diem (0-100): ");
            if (!double.TryParse(Console.ReadLine(), out double score) || score < 0 || score > 100)
            {
                Console.WriteLine("Diem nhap vao khong hop le.");
                return;
            }

            student.TakeExam(enrollments[index - 1].Course.Name, score);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Loi khi nhap diem: " + ex.Message);
        }
    }

    static void HienThiDanhSach()
    {
        if (students.Count == 0)
        {
            Console.WriteLine("Chua co hoc vien nao.");
            return;
        }
        foreach (var s in students)
        {
            s.DisplayInfo();
            Console.WriteLine();
        }
    }

    static void GhiDuLieuRaFile()
    {
        try
        {
            using (StreamWriter sw = new StreamWriter("students.csv"))
            {
                sw.WriteLine("FullName,Email,CourseName,CourseLevel,Score");

                foreach (var s in students)
                {
                    foreach (var e in s.GetEnrollments())
                    {
                        string line = $"{EscapeCsv(s.FullName)},{EscapeCsv(s.Email)},{EscapeCsv(e.Course.Name)},{e.Course.Level},{(e.Score < 0 ? "" : e.Score.ToString())}";
                        sw.WriteLine(line);
                    }
                }
            }
            Console.WriteLine("Ghi du lieu ra file thanh cong.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Loi khi ghi file: " + ex.Message);
        }
    }

    static void DocDuLieuTuFile()
    {
        try
        {
            if (!File.Exists("students.csv"))
            {
                Console.WriteLine("File students.csv khong ton tai.");
                return;
            }

            students.Clear();

            var lines = File.ReadAllLines("students.csv");
            for (int i = 1; i < lines.Length; i++)
            {
                var parts = SplitCsvLine(lines[i]);
                if (parts.Length < 5) continue;

                string fullName = parts[0];
                string email = parts[1];
                string courseName = parts[2];
                if (!Enum.TryParse(parts[3], out CourseLevel level))
                {
                    level = CourseLevel.Beginner;
                }
                double score = -1;
                if (!string.IsNullOrEmpty(parts[4]))
                {
                    double.TryParse(parts[4], out score);
                }

                var student = students.Find(s => s.Email.ToLower() == email.ToLower());
                if (student == null)
                {
                    student = new Student(fullName, email);
                    students.Add(student);
                }

                var course = courses.Find(c => c.Name == courseName);
                if (course == null)
                {
                    course = new Course(courseName, level);
                    courses.Add(course);
                }

                var enrollment = student.GetEnrollments().Find(e => e.Course.Name == courseName);
                if (enrollment == null)
                {
                    student.RegisterCourse(course);
                    enrollment = student.GetEnrollments().Find(e => e.Course.Name == courseName);
                }
                if (enrollment != null && score >= 0)
                {
                    enrollment.Score = score;
                }
            }

            Console.WriteLine("Doc du lieu tu file thanh cong.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Loi khi doc file: " + ex.Message);
        }
    }

    static string[] SplitCsvLine(string line)
    {
        var result = new List<string>();
        bool inQuotes = false;
        string value = "";
        foreach (char c in line)
        {
            if (c == '\"')
            {
                inQuotes = !inQuotes;
                continue;
            }
            if (c == ',' && !inQuotes)
            {
                result.Add(value);
                value = "";
            }
            else
            {
                value += c;
            }
        }
        result.Add(value);
        return result.ToArray();
    }

    static string EscapeCsv(string s)
    {
        if (s.Contains(","))
        {
            return "\"" + s + "\"";
        }
        return s;
    }
}
