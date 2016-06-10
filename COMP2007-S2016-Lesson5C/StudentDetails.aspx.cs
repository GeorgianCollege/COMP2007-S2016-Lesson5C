using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

// using statements required for EF DB access
using COMP2007_S2016_Lesson5C.Models;
using System.Web.ModelBinding;

namespace COMP2007_S2016_Lesson5C
{
    public partial class StudentDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if((!IsPostBack) && (Request.QueryString.Count > 0))
            {
                this.GetStudent();
            }
        }

        protected void GetStudent()
        {
            // populate teh form with existing data from the database
            int StudentID = Convert.ToInt32(Request.QueryString["StudentID"]);

            // connect to the EF DB
            using (DefaultConnection db = new DefaultConnection())
            {
                // populate a student object instance with the StudentID from the URL Parameter
                Student updatedStudent = (from student in db.Students
                                          where student.StudentID == StudentID
                                          select student).FirstOrDefault();

                // map the student properties to the form controls
                if(updatedStudent != null)
                {
                    LastNameTextBox.Text = updatedStudent.LastName;
                    FirstNameTextBox.Text = updatedStudent.FirstMidName;
                    EnrollmentDateTextBox.Text = updatedStudent.EnrollmentDate.ToString("yyyy-MM-dd");
                }
            }
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            // Redirect back to Students page
            Response.Redirect("~/Students.aspx");
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            // Use EF to connect to the server
            using (DefaultConnection db = new DefaultConnection())
            {
                // use the Student model to create a new student object and
                // save a new record
                Student newStudent = new Student();

                int StudentID = 0;

                if(Request.QueryString.Count > 0) // our URL has a StudentID in it
                {
                    // get the id from the URL
                    StudentID = Convert.ToInt32(Request.QueryString["StudentID"]);

                    // get the current student from EF DB
                    newStudent = (from student in db.Students
                                  where student.StudentID == StudentID
                                  select student).FirstOrDefault();
                }

                // add form data to the new student record
                newStudent.LastName = LastNameTextBox.Text;
                newStudent.FirstMidName = FirstNameTextBox.Text;
                newStudent.EnrollmentDate = Convert.ToDateTime(EnrollmentDateTextBox.Text);

                // use LINQ to ADO.NET to add / insert new student into the database

                if (StudentID == 0) {
                    db.Students.Add(newStudent);
                }
                

                // save our changes - also updates and inserts
                db.SaveChanges();

                // Redirect back to the updated students page
                Response.Redirect("~/Students.aspx");
            }
        }
    }
}