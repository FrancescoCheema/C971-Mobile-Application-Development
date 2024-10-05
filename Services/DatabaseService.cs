using C971.Models;
using SQLite;
using System;
using System.Collections.ObjectModel;
using static SQLite.SQLite3;

namespace C971.Services
{
    public class DatabaseService
    {
        const string DatabaseFileName = "c971data.db";

        static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFileName);
        const SQLiteOpenFlags Flags = SQLite.SQLiteOpenFlags.ReadWrite | SQLite.SQLiteOpenFlags.Create | SQLite.SQLiteOpenFlags.SharedCache;
        SQLiteAsyncConnection Database;
        public async Task Init()
        {
            if (Database is not null)
            {
                return;
            }

            Database = new(DatabasePath);

            var result = await Database.CreateTableAsync<Terms>();
            Console.WriteLine($"Terms table result: {result}");

            var coursesResult = await Database.CreateTableAsync<Courses>();
            Console.WriteLine($"Courses table result: {coursesResult}");

            var assessmentsResult = await Database.CreateTableAsync<Assessments>();
            Console.WriteLine($"Assessments table result: {assessmentsResult}");

            var userResult = await Database.CreateTableAsync<UserCredential>();
            Console.WriteLine($"Assessments table result: {userResult}");
        }
       
        public async Task<int> AddUser(string username, string password)
        {
            await Init();
            var user = new UserCredential
            {
                Username = username,
                Password = password
            };
            var result = await Database.InsertAsync(user);
            Console.WriteLine($"AddUser result: {result}");
            return result;
        }

        public async Task<UserCredential> GetUser(string username, string password)
        {
            await Init();
            return await Database.Table<UserCredential>()
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower() && u.Password == password);
        }

        public async Task<List<UserCredential>> GetAllUsers()
        {
            await Init();
            return await Database.Table<UserCredential>().ToListAsync();
        }

        public async Task<int> UpdateAssessmentAsync(Assessments assessment)
        {
            await Init();
            return await Database.UpdateAsync(assessment);
        }

        public async Task DropTableAsync<T>()
        {
            await Database.ExecuteAsync($"DROP TABLE IF EXISTS {typeof(T).Name}");
        }

        public async Task<List<Terms>> GetTerms()
        {
            await Init();
            return await Database.Table<Terms>().ToListAsync();
        }

        public async Task<List<Courses>> GetCourses()
        {
            await Init();
            return await Database.Table<Courses>().ToListAsync();
        }

        public async Task<List<Assessments>> GetAssessments()
        {
            await Init();
            return await Database.Table<Assessments>().ToListAsync();
        }

        public async Task<Terms> GetTermsAsync(int termId)
        {
            await Init();
            return await Database.Table<Terms>().Where(i => i.termId == termId).FirstOrDefaultAsync();
        }

        public async Task<int> AddTermAsync(Terms terms)
        {
            await Init();
            if (terms.termId != 0)
            {
                return await Database.UpdateAsync(terms);
            }
            else
            {
                return await Database.InsertAsync(terms);
            }
        }

        public async Task<int> AddCourseAsync(Courses course)
        {
            await Init();

            if (course != null)
            {
                await Database.InsertAsync(course);
            }

            return 0;
        }

        public async Task<int> AddAssessmentAsync(Assessments assessment)
        {
            await Init();

            int result;
            if (assessment.CourseId > 0)
            {
                result = await Database.UpdateAsync(assessment);
            }
            else
            {
                result = await Database.InsertAsync(assessment);
            }

            return result;
        }

        public async Task<int> InsertAssessmentAsync(Assessments assessment)
        {
            await Init();

            int result;
         
                result = await Database.InsertAsync(assessment);
           
                return 0;
        }

        public async Task<IEnumerable<Assessments>> GetAssessmentList(int courseId)
        {
            await Init();

            var assessmentFiltered = await Database.Table<Assessments>()
                .Where(a => a.CourseId == courseId && !string.IsNullOrEmpty(a.PerformanceAssessmentName))
                .ToListAsync();

            if (assessmentFiltered == null || !assessmentFiltered.Any())
            {
                Console.WriteLine($"No assessments found for CourseId: {courseId}");
                return new ObservableCollection<Assessments>();
            }
            else
            {
                Console.WriteLine($"Found {assessmentFiltered.Count} assessments for CourseId: {courseId}");
            }

            return new ObservableCollection<Assessments>(assessmentFiltered);
        }

        public async Task<IEnumerable<Assessments>> GetOBAssessmentList(int courseId)
        {
            await Init();

            var assessmentFiltered = await Database.Table<Assessments>()
               .Where(a => a.CourseId == courseId && !string.IsNullOrEmpty(a.ObjectiveAssessmentName))
               .ToListAsync();

            if (assessmentFiltered == null || !assessmentFiltered.Any())
            {
                Console.WriteLine($"No assessments found for CourseId: {courseId}");
                return new ObservableCollection<Assessments>();
            }
            else
            {
                Console.WriteLine($"Found {assessmentFiltered.Count} assessments for CourseId: {courseId}");
            }

            return new ObservableCollection<Assessments>(assessmentFiltered);
        }

        public async Task<Terms> ViewTermInDatabase(int termId)
        {
            await Init();
            return await Database.Table<Terms>().Where(t => t.termId == termId).FirstOrDefaultAsync();
        }

        public async Task<Courses> ViewCoursesInDatabase(int termId)
        {
            await Init();
            return await Database.Table<Courses>().Where(c => c.courseId == termId).FirstOrDefaultAsync();
        }

        public async Task<List<Courses>> GetCoursesForTermAsync(int termId)
        {
            await Init();
            var courses = await Database.Table<Courses>().Where(c => c.courseId == termId).ToListAsync();
            return courses;
        }

        public async Task UpdateTermAsync(Terms term)
        {
            await Init();
            await Database.UpdateAsync(term);
        }

        public Task<int> GetTask(int termId)
        {
            return Database.Table<Courses>().Where(c => c.courseId == termId).DeleteAsync();
        }

        public async Task DeleteCourse(int courseId)
        {
            var courseToDelete = await Database.Table<Courses>().FirstOrDefaultAsync(c => c.CourseId == courseId);
            if (courseToDelete != null)
            {
                await Database.DeleteAsync(courseToDelete);
            }
        }

        public async Task DeleteTerm(int termId)
        {
            await Init();
            var term = await Database.Table<Terms>().FirstOrDefaultAsync(t => t.termId == termId);
            if (term != null)
            {
                await Database.DeleteAsync(term);
            }
        }

        public async Task DeleteAssessment(int courseId, int assessmentId)
        {
            await Init();
            var assessment = await Database.Table<Assessments>()
            .FirstOrDefaultAsync(a => a.CourseId == courseId && a.assessmentId == assessmentId);
            if (assessment != null)
            {
                await Database.DeleteAsync(assessment);
            }
        }
    }
}
