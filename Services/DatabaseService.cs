using C971.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C971.Services
{
    public class DatabaseService
    {
        const string DatabaseFileName = "c971data.db";

        static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFileName);
        const SQLite.SQLiteOpenFlags Flags = SQLite.SQLiteOpenFlags.ReadWrite | SQLite.SQLiteOpenFlags.Create | SQLite.SQLiteOpenFlags.SharedCache;
        SQLiteAsyncConnection Database;

        public async Task Init()
        {
            if (Database is not null)
            {
                return;
            }

            Database = new(DatabasePath);


            var result = await Database.CreateTableAsync<Terms>();
            var coursesResult = await Database.CreateTableAsync<Courses>();
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
            if (course.CourseId != 0)
            {
                return await Database.UpdateAsync(course);
            }
            else
            {
                return await Database.InsertAsync(course);
            }

        }

        public async Task<Terms> ViewTermInDatabase(int termId)
        {
            await Init();
            return await Database.Table<Terms>().Where(t => t.termId == termId).FirstOrDefaultAsync();
        }

        public async Task<Courses> ViewCoursesInDatabase(int termId)
        {
            await Init();
            return await Database.Table<Courses>().Where(c => c.CourseId == termId).FirstOrDefaultAsync();
        }

        public async Task<List<Courses>> GetCoursesForTermAsync(int termId)
        {
            await Init();
            var courses = await Database.Table<Courses>().Where(c => c.courseId == termId).ToListAsync();
            return courses;
        }

        public Task<int> GetTask(int termId)
        {
            return Database.Table<Courses>().Where(c => c.CourseId == termId).DeleteAsync();
        }

        public async Task DeleteCourse(int courseId)
        {
            await Init();
            var course = await Database.Table<Courses>().FirstOrDefaultAsync(c => c.CourseId == courseId);
            if (course != null)
            {
                await Database.DeleteAsync(course);
            }
        }
    }
}
