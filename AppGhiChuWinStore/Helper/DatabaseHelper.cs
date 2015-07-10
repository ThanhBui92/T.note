using AppGhiChuWinStore.DataModel;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppGhiChuWinStore.Helper
{
    class DatabaseHelper
    {
        SQLiteConnection dbConn;

        //Create Tabble 
        public void onCreate(string DB_PATH)
        {
            try
            {
                if (!CheckFileExists(DB_PATH).Result)
                {
                    using (dbConn = new SQLiteConnection(DB_PATH))
                    {
                        dbConn.CreateTable<GhiChu>();
                    }
                }
            }
            catch
            {
            }
        }
        private async Task<bool> CheckFileExists(string fileName)
        {
            try
            {
                var store = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public ObservableCollection<GhiChu> SearchContacts(string key)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                List<GhiChu> myCollection = dbConn.Query<GhiChu>("select * from GhiChu where Content LIKE '%" + key + "%' OR Title LIKE '%" + key + "%'").ToList<GhiChu>();
                ObservableCollection<GhiChu> ContactsList = new ObservableCollection<GhiChu>(myCollection);
                return ContactsList;
            }
        }
        public bool CheckContactExist(string key)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                List<GhiChu> myCollection = dbConn.Query<GhiChu>("select * from GhiChu where Title LIKE '" + key + "' and Complete = 0").ToList<GhiChu>();
                ObservableCollection<GhiChu> ContactsList = new ObservableCollection<GhiChu>(myCollection);
                if (ContactsList.Count != 0)
                    return true;
                else
                    return false;
            }
        }
        // Retrieve the specific contact from the database. 
        public GhiChu ReadContact(int contactid)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var existingconact = dbConn.Query<GhiChu>("select * from GhiChu where Id =" + contactid).FirstOrDefault();
                return existingconact;
            }
        }
        public ObservableCollection<GhiChu> ReadAllContacts()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                List<GhiChu> myCollection = dbConn.Query<GhiChu>("select * from GhiChu ORDER BY id DESC").ToList<GhiChu>();
                ObservableCollection<GhiChu> ContactsList = new ObservableCollection<GhiChu>(myCollection);
                return ContactsList;
            }
        }
        // Retrieve the all contact list from the database. 
        public ObservableCollection<GhiChu> ReadContacts()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                List<GhiChu> myCollection = dbConn.Query<GhiChu>("select * from GhiChu where Complete = 0 ORDER BY id DESC").ToList<GhiChu>();
                ObservableCollection<GhiChu> ContactsList = new ObservableCollection<GhiChu>(myCollection);
                return ContactsList;
            }
        }
        public ObservableCollection<GhiChu> ReadContactsComplete()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                List<GhiChu> myCollection = dbConn.Query<GhiChu>("select * from GhiChu where Complete = 1 ORDER BY id DESC").ToList<GhiChu>();
                ObservableCollection<GhiChu> ContactsList = new ObservableCollection<GhiChu>(myCollection);
                return ContactsList;
            }
        }

        //Update existing conatct 
        public void UpdateContact(GhiChu user)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var existingconact = dbConn.Query<GhiChu>("select * from GhiChu where Id =" + user.Id).FirstOrDefault();
                if (existingconact != null)
                {
                    existingconact.Title = user.Title;
                    existingconact.Content = user.Content;
                    existingconact.Time = user.Time;
                    existingconact.Complete = user.Complete;
                    existingconact.Remind = user.Remind;
                    dbConn.RunInTransaction(() =>
                    {
                        dbConn.Update(existingconact);
                    });
                }
            }
        }
        // Insert the new contact in the Contacts table. 
        public void Insert(GhiChu newcontact)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                dbConn.RunInTransaction(() =>
                {
                    dbConn.Insert(newcontact);
                });
            }
        }

        //Delete specific contact 
        public void DeleteContact(int Id)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var existingconact = dbConn.Query<GhiChu>("select * from GhiChu where Id =" + Id).FirstOrDefault();
                if (existingconact != null)
                {
                    dbConn.RunInTransaction(() =>
                    {
                        dbConn.Delete(existingconact);
                    });
                }
            }
        }
        //Delete all contactlist or delete Contacts table 
        public void DeleteAllContact()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                //dbConn.RunInTransaction(() => 
                //   { 
                dbConn.DropTable<GhiChu>();
                dbConn.CreateTable<GhiChu>();
                dbConn.Dispose();
                dbConn.Close();
                //}); 
            }
        }
    }
}
