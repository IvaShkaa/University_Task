using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using MySql.Data.MySqlClient;

class Program
{
    static void Main(string[] args)
    {
        // Строка подключения к базе данных
        string connectionString = "Server=localhost;User ID=root;Password=12345678;";

        using (var connection = new MySqlConnection(connectionString))
        {
            string command = "";
            connection.Open();
            
            while (command != "СТОП")
            {
                Console.WriteLine(@"Добро пожаловать! Вот список доступных команд:
                                    
                                    СОЗДАТЬ - создаёт новую бд
                                    ДОБАВИТЬ - добавление новго студента\учителя\курса\экзамена\оценки
                                    ИЗМЕНИТЬ - изменить информацию о студенте\учителе\курсе
                                    УДАЛИТЬ - удалить информацию о студенте\учителе\курсе\экзамене
                                    
                                    СТУДПОФАК - Получение списка студентов по факультету
                                    КУРСЧИТПРЕД - Получение списка курсов, читаемых определенным преподавателем
                                    ОЦЕНПОКУРС - Получение оценок студентов по определенному курсу
                                    СРЕДБАЛЛ - Средний балл студента в целом
                                    СРЕДБАЛЛПОФАК - Средний балл по факультету

                                    СТОП - остановка программы
                                    ");

                command = Console.ReadLine();
                if (command == "СОЗДАТЬ")
                {
                    CreateAll(connection);
                }    

                else if (command == "ДОБАВИТЬ")
                {
                    Console.WriteLine("Напишите кого добавим");
                    Console.WriteLine("студента, учителя, курс, экзамен, оценку ?");
                    command = Console.ReadLine();
                    while (true)
                    {
                        bool a = Insert(connection, command);
                        if (a = true)
                        {
                            break;
                        }
                    }
                } 
                
                else if (command == "ИЗМЕНИТЬ")
                {
                    Console.WriteLine("Напишите кого изменим");
                    Console.WriteLine("студента, учителя, курс ?");
                    command = Console.ReadLine();
                    while (true)
                    {
                        bool a = Update(connection, command);
                        if (a = true)
                        {
                            break;
                        }
                    }
                }


            }


            connection.Close();
            Console.WriteLine("Соединение с базой данных закрыто.");
        }
    }
    
    static void CreateAll(MySqlConnection connection)
    {
        CreateTableStudents(connection);
        CreateTableTeachers(connection);
        CreateTableCourses(connection);
        CreateTableExams(connection);
        CreateTableGrades(connection);

        Console.WriteLine("Все таблицы успешно созданы");
    }

    static void CreateTableStudents(MySqlConnection connection)
    {
        using (var command = new MySqlCommand("", connection))
        {
            command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Students (
                student_id INT AUTO_INCREMENT PRIMARY KEY,
                name VARCHAR(100) NOT NULL,
                surname VARCHAR(100) NOT NULL,
                depatment VARCHAR(100) NOT NULL,
                date_of_birth DATETIME NOT NULL
            );";
            command.ExecuteNonQuery();
            Console.WriteLine("Таблица 'Students' успешно создана.");
        }
    }

    static void CreateTableTeachers(MySqlConnection connection)
    {
        using (var command = new MySqlCommand("", connection))
        {
            command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Teachers (
                teacher_id INT AUTO_INCREMENT PRIMARY KEY,
                name VARCHAR(100) NOT NULL,
                surname VARCHAR(100) NOT NULL,
                depatment VARCHAR(100) NOT NULL
            );";
            command.ExecuteNonQuery();
            Console.WriteLine("Таблица 'Teachers' успешно создана.");
        }
    }

    static void CreateTableCourses(MySqlConnection connection)
    {
        using (var command = new MySqlCommand("", connection))
        {
            command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Courses (
                course_id INT AUTO_INCREMENT PRIMARY KEY,
                title VARCHAR(100) NOT NULL,
                description VARCHAR(150) NOT NULL,
                teacher_id INT AUTO_INCREMENT FOREIGN KEY
            );";
            command.ExecuteNonQuery();
            Console.WriteLine("Таблица 'Courses' успешно создана.");
        }
    }

    static void CreateTableExams(MySqlConnection connection)
    {
        using (var command = new MySqlCommand("", connection))
        {
            command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Exams (
                exam_id INT AUTO_INCREMENT PRIMARY KEY,
                date DATETIME NOT NULL,
                course_id INT AUTO_INCREMENT FOREIGN KEY,
                max_score INT NOT NULL
            );";
            command.ExecuteNonQuery();
            Console.WriteLine("Таблица 'Exams' успешно создана.");
        }
    }

    static void CreateTableGrades(MySqlConnection connection)
    {
        using (var command = new MySqlCommand("", connection))
        {
            command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Grades (
                id INT AUTO_INCREMENT PRIMARY KEY,
                student_id INT AUTO_INCREMENT FOREIGN KEY,
                exam_id INT AUTO_INCREMENT FOREIGN KEY,
                Score INT NOT NULL
            );";
            command.ExecuteNonQuery();
            Console.WriteLine("Таблица 'Grades' успешно создана.");
        }
    }



    static bool Insert(MySqlConnection connection, string com)
    {
        bool is_done = true;
        if (com == "студента")
        {
            Console.WriteLine("Введите имя студента");
            string name = Console.ReadLine();
            Console.WriteLine("Введите фамилию студента");
            string surname = Console.ReadLine();
            Console.WriteLine("Введите факультет");
            string department = Console.ReadLine();
            Console.WriteLine("Введите дату рождения");
            string date = Console.ReadLine();
            InsertStudent(connection, name, surname, department, date);
            return is_done;
        }

        else if (com == "учителя")
        {
            Console.WriteLine("Введите имя учителя");
            string name = Console.ReadLine();
            Console.WriteLine("Введите фамилию учителя");
            string surname = Console.ReadLine();
            Console.WriteLine("Введите кафедру");
            string department = Console.ReadLine();
            InsertTeacher(connection, name, surname, department);
            return is_done;
        }

        else if (com == "курс")
        {
            Console.WriteLine("Введите название");
            string title = Console.ReadLine();
            Console.WriteLine("Введите описание");
            string description = Console.ReadLine();
            Console.WriteLine("Введите id учителя");
            int teacher_id = int.Parse(Console.ReadLine());
            InsertCourse(connection, title, description, teacher_id);
            return is_done;
        }

        else if (com == "экзамен")
        {
            Console.WriteLine("Введите дату");
            string date = Console.ReadLine();
            Console.WriteLine("Введите id предмета");
            int teacher_id = int.Parse(Console.ReadLine());
            Console.WriteLine("Введите максимальный балл");
            int max_score = int.Parse(Console.ReadLine());
            InsertExam(connection, date, teacher_id, max_score);
            return is_done;
        }

        else if (com == "оценку")
        {
            Console.WriteLine("Введите id студента");
            int stident_id = int.Parse(Console.ReadLine());
            Console.WriteLine("Введите id экзамена");
            int exam_id = int.Parse(Console.ReadLine());
            Console.WriteLine("Введите оценку");
            int score = int.Parse(Console.ReadLine());
            InsertGrade(connection, stident_id, exam_id, score);
            return is_done;
        }

        else
        {
            Console.WriteLine("Некорректный ввод");
            is_done = false;
            return is_done;
        }
    }

    static void InsertStudent(MySqlConnection connection, string name, string surname, string department, string date_of_birth)
    {
        using (var command = new MySqlCommand("", connection))
        {
            command.CommandText = "INSERT INTO Students (name, surname, department, date_of_birth) VALUES (@name, @surname, @department, @date_of_birth)";
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@surname", surname);
            command.Parameters.AddWithValue("@department", department);
            command.Parameters.AddWithValue("@date_of_birth", date_of_birth);
            command.ExecuteNonQuery();
            Console.WriteLine($"Студент {name} {surname} добавлен.");
        }
    }

    static void InsertTeacher(MySqlConnection connection, string name, string surname, string department)
    {
        using (var command = new MySqlCommand("", connection))
        {
            command.CommandText = "INSERT INTO Teachers (name, surname, department) VALUES (@name, @surname, @department)";
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@surname", surname);
            command.Parameters.AddWithValue("@department", department);
            command.ExecuteNonQuery();
            Console.WriteLine($"Преподаватель {name} {surname} добавлен.");
        }
    }

    static void InsertCourse(MySqlConnection connection, string title, string description, int teacher_id)
    {
        using (var command = new MySqlCommand("", connection))
        {
            command.CommandText = "INSERT INTO Courses (title, description, teacher_id) VALUES (@title, @description, @teacher_id)";
            command.Parameters.AddWithValue("@title", title);
            command.Parameters.AddWithValue("@description", description);
            command.Parameters.AddWithValue("@teacher_id", teacher_id);
            command.ExecuteNonQuery();
            Console.WriteLine($"Курс {title} добавлен.");
        }
    }

    static void InsertExam(MySqlConnection connection, string date, int course_id, int max_score)
    {
        using (var command = new MySqlCommand("", connection))
        {
            command.CommandText = "INSERT INTO Exams (date, course_id, max_score) VALUES (@date, @course_id, @max_score)";
            command.Parameters.AddWithValue("@date", date);
            command.Parameters.AddWithValue("@course_id", course_id);
            command.Parameters.AddWithValue("@max_score", max_score);
            command.ExecuteNonQuery();
            Console.WriteLine($"Экзамен по предмету {course_id} добавлен.");
        }
    }

    static void InsertGrade(MySqlConnection connection, int student_id, int exam_id, int score)
    {
        using (var command = new MySqlCommand("", connection))
        {
            command.CommandText = "INSERT INTO Grades (student_id, exam_id, score) VALUES (@student_id, @exam_id, @score)";
            command.Parameters.AddWithValue("@exam_id", exam_id);
            command.Parameters.AddWithValue("@student_id", student_id);
            command.Parameters.AddWithValue("@score", score);
            command.ExecuteNonQuery();
            Console.WriteLine($"Оценка {score} добавлена.");
        }
    }



    static bool Update(MySqlConnection connection, string com)
    {
        bool is_done = true;
        if (com == "студента")
        {
            Console.WriteLine("Введите id обновляемого студента");
            int student_id = int.Parse(Console.ReadLine());
            Console.WriteLine("Введите имя студента");
            string name = Console.ReadLine();
            Console.WriteLine("Введите фамилию студента");
            string surname = Console.ReadLine();
            Console.WriteLine("Введите факультет");
            string department = Console.ReadLine();
            Console.WriteLine("Введите дату рождения");
            string date = Console.ReadLine();
            UpdateStudent(connection, student_id, name, surname, department, date);
            return is_done;
        }

        else if (com == "учителя")
        {
            Console.WriteLine("Введите id обновляемого учителя");
            int teacher_id = int.Parse(Console.ReadLine());
            Console.WriteLine("Введите имя учителя");
            string name = Console.ReadLine();
            Console.WriteLine("Введите фамилию учителя");
            string surname = Console.ReadLine();
            Console.WriteLine("Введите кафедру");
            string department = Console.ReadLine();
            UpdateTeacher(connection, teacher_id, name, surname, department);
            return is_done;
        }

        else if (com == "курс")
        {
            Console.WriteLine("Введите id обновляемого курса");
            int course_id = int.Parse(Console.ReadLine());
            Console.WriteLine("Введите название");
            string title = Console.ReadLine();
            Console.WriteLine("Введите описание");
            string description = Console.ReadLine();
            Console.WriteLine("Введите id учителя");
            int teacher_id = int.Parse(Console.ReadLine());
            UpdateCourse(connection, course_id, title, description, teacher_id);
            return is_done;
        }

        else
        {
            Console.WriteLine("Некорректный ввод");
            is_done = false;
            return is_done;
        }

    }

    static void UpdateStudent(MySqlConnection connection, int student_id, string name, string surname, string department, string date_of_birth)
    {
        using (var command = new MySqlCommand("", connection))
        {
            command.CommandText = @"UPDATE Students SET name = @name WHERE student_id = @student_id
                                    UPDATE Students SET surname = @surname WHERE student_id = @student_id
                                    UPDATE Students SET Department = @department WHERE student_id = @student_id
                                    UPDATE Students SET date_of_birth = @dete_of_birth WHERE student_id = @student_id";

            command.Parameters.AddWithValue("@student_id", student_id);
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@surname", surname);
            command.Parameters.AddWithValue("@Department", department);
            command.Parameters.AddWithValue("@date_of_birth", date_of_birth);
            command.ExecuteNonQuery();
            Console.WriteLine($"Стуент '{name}' успешно обновлен.");
        }
    }

    static void UpdateTeacher(MySqlConnection connection, int teacher_id, string name, string surname, string department)
    {
        using (var command = new MySqlCommand("", connection))
        {
            command.CommandText = @"UPDATE Teachers SET name = @name WHERE teacher_id = @teacher_id
                                    UPDATE Teachers SET surname = @surname WHERE teacher_id = @teacher_id
                                    UPDATE Teachers SET Department = @department WHERE reacher_id = @teacher_id";

            command.Parameters.AddWithValue("@teacher_id", teacher_id);
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@surname", surname);
            command.Parameters.AddWithValue("@Department", department);
            command.ExecuteNonQuery();
            Console.WriteLine($"Учитель '{name}' успешно обновлен.");
        }
    }

    static void UpdateCourse(MySqlConnection connection, int course_id, string title, string description, int teacher_id)
    {
        using (var command = new MySqlCommand("", connection))
        {
            command.CommandText = @"UPDATE Courses SET title = @title WHERE course_id = @course_id
                                    UPDATE Courses SET description = @surname WHERE course_id = @course_id
                                    UPDATE Courses SET teacher_id = @department WHERE course_id = @course_id";

            command.Parameters.AddWithValue("@course_id", course_id);
            command.Parameters.AddWithValue("@title", title);
            command.Parameters.AddWithValue("@description", description);
            command.Parameters.AddWithValue("@teacher_id", teacher_id);
            command.ExecuteNonQuery();
            Console.WriteLine($"Курс '{title}' успешно обновлен.");
        }
    }



    static bool Delete(MySqlConnection connection, string com)
    {
        bool is_done = true;
        if (com == "студента")
        {
            Console.WriteLine("Введите id удаляемого студента");
            int ID = int.Parse(Console.ReadLine());
            DeleteStudent(connection, ID);
            return is_done;
        }

        else if (com == "учителя")
        {
            Console.WriteLine("Введите id удаляемого учителя");
            int ID = int.Parse(Console.ReadLine());
            DeleteStudent(connection, ID);
            return is_done;
        }

        else if (com == "курс")
        {
            Console.WriteLine("Введите id удаляемого курса");
            int ID = int.Parse(Console.ReadLine());
            DeleteStudent(connection, ID);
            return is_done;
        }

        else if (com == "студента")
        {
            Console.WriteLine("Введите id удаляемого студента");
            int ID = int.Parse(Console.ReadLine());
            DeleteStudent(connection, ID);
            return is_done;
        }
    }

    static void DeleteStudent(MySqlConnection connection, int student_id)
    {
        using (var command = new MySqlCommand("", connection))
        {
            command.CommandText = "DELETE FROM Students WHERE student_id = @student_id";
            command.Parameters.AddWithValue("@student_id", student_id);
            command.ExecuteNonQuery();
            Console.WriteLine($"Cтудент № '{student_id}' успешно удален.");
        }
    }

    static void DeleteTeacher(MySqlConnection connection, int teacher_id)
    {
        using (var command = new MySqlCommand("", connection))
        {
            command.CommandText = "DELETE FROM Teachers WHERE teacher_id = @teacher_id";
            command.Parameters.AddWithValue("@teacher_id", teacher_id);
            command.ExecuteNonQuery();
            Console.WriteLine($"Учитель № '{teacher_id}' успешно удален.");
        }
    }

    static void DeleteCourse(MySqlConnection connection, int course_id)
    {
        using (var command = new MySqlCommand("", connection))
        {
            command.CommandText = "DELETE FROM Courses WHERE course_id = @course_id";
            command.Parameters.AddWithValue("@course_id", course_id);
            command.ExecuteNonQuery();
            Console.WriteLine($"Курс № '{course_id}' успешно удален.");
        }
    }

    static void DeleteExam(MySqlConnection connection, int exam_id)
    {
        using (var command = new MySqlCommand("", connection))
        {
            command.CommandText = "DELETE FROM Exams WHERE exam_id = @exam_id";
            command.Parameters.AddWithValue("@exam_id", exam_id);
            command.ExecuteNonQuery();
            Console.WriteLine($"Экзамен № '{exam_id}' успешно удален.");
        }
    }



    static void GetStudentsByDepartment(MySqlConnection connection, string department)
    {
        using (var command = new MySqlCommand("SELECT name, surname FROM Student WHERE deparment = @department", connection))
        using (var reader = command.ExecuteReader())
        {
            command.Parameters.AddWithValue("@department", department);
            while (reader.Read())
            {
                Console.WriteLine($"ID: {reader["student_id"]}, Имя: {reader["name"]}, Фамилия: {reader["surname"]}");
            }
        }
    }

    static void GetCourseByTeacher(MySqlConnection connection, int teacher_id)
    {
        using (var command = new MySqlCommand("SELECT title FROM Courses WHERE teacher_id = @teacher_id", connection))
        using (var reader = command.ExecuteReader())
        {
            command.Parameters.AddWithValue("@teacher_id", teacher_id);
            while (reader.Read())
            {
                Console.WriteLine($"ID: {reader["course_id"]}, Название: {reader["title"]}");
            }
        }
    }

     
   static void GetAvgScoreOfStudent(MySqlConnection connection, int student_id)
    {
        using (var command = new MySqlCommand("", connection))
        using (var reader = command.ExecuteReader())
        {
            command.CommandText = @"SELECT student_id, avg(score)
                                    FROM Grades
                                    GROUP BY Grades.student_id
                                    WHERE Grades.student_id = @student_id"
;
            command.Parameters.AddWithValue("@student_id", student_id);
            while (reader.Read())
            {
                Console.WriteLine($"ID: {student_id}, Средний балл: {reader[""]}");
            }
        }
    }

    static void GetAvgScoreByCourse(MySqlConnection connection, int course_id)
    {
        using (var command = new MySqlCommand("", connection))
        using (var reader = command.ExecuteReader())
        {
            command.CommandText = @"SELECT course_id, avg(score)
                                    FROM Grades
                                    GROUP BY Grades.course_id
                                    WHERE Grades.course_id = @course_id"
;
            command.Parameters.AddWithValue("@course_id", course_id);
            while (reader.Read())
            {
                Console.WriteLine($"ID: {course_id}, Средний балл: {reader[""]}");
            }
        }
    }

    static void GetGradesByCourse(MySqlConnection connection, int course_id)
    {
        using (var command = new MySqlCommand("SELECT score FROM Courses WHERE course_id = @course_id", connection))
        using (var reader = command.ExecuteReader())
        {
            command.Parameters.AddWithValue("@course_id", course_id);
            while (reader.Read())
            {
                Console.WriteLine($"ID: {reader["student_id"]}, Название: {reader["score"]}");
            }
        }
    }

}