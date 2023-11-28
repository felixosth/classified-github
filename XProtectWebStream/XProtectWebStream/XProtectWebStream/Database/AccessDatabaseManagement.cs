using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XProtectWebStream.Database.Objects;

namespace XProtectWebStream.Database
{
    internal class AccessDatabaseManagement
    {
        LocalDatabase localDatabase;

        internal event EventHandler<string> OnLog;

        //internal List<AccessUser> AccessUsers { get; private set; }
        internal IEnumerable<AccessGroup> AccessGroups { get; private set; }
        private object accessGroupsLock = new object();

        internal event EventHandler<AccessGroupsUpdatedEventArgs> AccessGroupsUpdated;

        internal AccessDatabaseManagement(LocalDatabase localDatabase)
        {
            this.localDatabase = localDatabase;

            AccessGroups = GetAccessGroups(includeUnassigned: false, includeAnyone: false);
        }

        private void UpdateAccessGroups(bool shouldUpdateClients)
        {
            ThreadPool.QueueUserWorkItem((state) =>
            {
                lock(accessGroupsLock)
                {
                    AccessGroups = GetAccessGroups(includeUnassigned: false, includeAnyone: false);
                }
                AccessGroupsUpdated?.Invoke(this, new AccessGroupsUpdatedEventArgs(AccessGroups, shouldUpdateClients));
            });
        }

        internal IEnumerable<AccessGroup> GetAccessGroups(bool includeUnassigned, bool includeAnyone = false)
        {
            List<AccessUser> users = new List<AccessUser>();
            users.AddRange(GetAccessUsers());

            List<AccessGroup> accessGroups = new List<AccessGroup>();
            using (var command = localDatabase.GetCommand())
            {
                command.CommandText = "SELECT id, name FROM accessgroups;";

                try
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var grp = new AccessGroup(reader.GetInt32(0), reader.GetString(1));
                            grp.Users = users.Where(u => u.AccessGroupId == grp.Id).ToList();
                            users.RemoveAll(u => grp.Users.Contains(u));

                            accessGroups.Add(grp);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log("Error selecting accessgroups: " + ex.Message);
                }
            }

            if (includeUnassigned)
                accessGroups.Insert(0, new AccessGroup(AccessGroup.UnassignedGroupId, "Unassigned") // Used for web
                {
                    Users = users
                });

            if (includeAnyone)
                accessGroups.Insert(0, new AccessGroup(AccessGroup.AnonGroupId, "Anyone"));

            return accessGroups;
        }

        internal bool InsertAccessGroup(AccessGroup accessGroup)
        {
            using (var command = localDatabase.GetCommand())
            {
                command.CommandText = "INSERT INTO accessgroups (name) VALUES ($name);";
                command.Parameters.AddWithValue("$name", accessGroup.Name);
                if (command.ExecuteNonQuery() > 0)
                {
                    UpdateAccessGroups(shouldUpdateClients: true);
                    return true;
                }
                return false;
            }
        }

        internal bool ModifyAccessGroup(AccessGroup accessGroup)
        {
            using (var command = localDatabase.GetCommand())
            {
                command.CommandText = "UPDATE accessgroups SET name=$name WHERE id=$id;";
                command.Parameters.AddWithValue("$name", accessGroup.Name);
                command.Parameters.AddWithValue("$id", accessGroup.Id);
                if (command.ExecuteNonQuery() > 0)
                {
                    UpdateAccessGroups(shouldUpdateClients: true);
                    return true;
                }
                return false;
            }
        }

        internal bool DeleteAccessGroup(int id)
        {
            using (var command = localDatabase.GetCommand())
            {
                command.CommandText = "DELETE FROM accessgroups WHERE id=$id;";
                command.Parameters.AddWithValue("$id", id);
                if (command.ExecuteNonQuery() > 0)
                {
                    UpdateAccessGroups(shouldUpdateClients: true);
                    return true;
                }
                return false;
            }
        }

        internal IEnumerable<AccessUser> GetAccessUsers()
        {
            List<AccessUser> accessUsers = new List<AccessUser>();
            using (var command = localDatabase.GetCommand())
            {
                command.CommandText = "SELECT id, accessgroupid, name, pnr FROM accessusers;";

                try
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            accessUsers.Add(new AccessUser(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2), reader.GetString(3)));
                        }
                    }
                }
                catch(Exception ex)
                {
                    Log("Error selecting accessusers: " + ex.Message);
                }
            }
            return accessUsers;
        }

        internal bool InsertAccessUser(AccessUser accessUser)
        {
            using (var command = localDatabase.GetCommand())
            {
                command.CommandText = "INSERT INTO accessusers (accessgroupid, name, pnr) VALUES ($accessgroupid, $name, $pnr);";
                command.Parameters.AddWithValue("$accessgroupid", accessUser.AccessGroupId);
                command.Parameters.AddWithValue("$name", accessUser.Name);
                command.Parameters.AddWithValue("$pnr", accessUser.PNR);
                return command.ExecuteNonQuery() > 0;
            }
        }

        internal bool ModifyAccessUser(AccessUser accessUser)
        {
            using (var command = localDatabase.GetCommand())
            {
                command.CommandText = "UPDATE accessusers SET accessgroupid=$accessgroupid, name=$name WHERE id=$id;";
                command.Parameters.AddWithValue("$accessgroupid", accessUser.AccessGroupId);
                command.Parameters.AddWithValue("$name", accessUser.Name);
                //command.Parameters.AddWithValue("$pnr", accessUser.PNR);
                command.Parameters.AddWithValue("$id", accessUser.Id);
                if(command.ExecuteNonQuery() > 0)
                {
                    UpdateAccessGroups(shouldUpdateClients: false);
                    return true;
                }
                return false;
            }
        }

        internal bool DeleteAccessUser(int id)
        {
            using (var command = localDatabase.GetCommand())
            {
                command.CommandText = "DELETE FROM accessusers WHERE id=$id;";
                command.Parameters.AddWithValue("$id", id);
                if (command.ExecuteNonQuery() > 0)
                {
                    UpdateAccessGroups(shouldUpdateClients: false);
                    return true;
                }
                return false;
            }
        }

        private void Log(string msg)
        {
            OnLog?.Invoke(this, msg);
        }

        internal bool UserHaveAccessTo(int[] AccessGroupIds, string pnr)
        {
            if (string.IsNullOrWhiteSpace(pnr))
                return false;

            if (AccessGroupIds.Length == 0 || AccessGroupIds.Contains(AccessGroup.AnonGroupId))
                return true;

            lock (accessGroupsLock)
            {
                foreach (var grp in AccessGroups.Where(agrp => AccessGroupIds.Contains(agrp.Id)))
                {
                    if (grp.Users.Any(usr => usr.MatchPNR(pnr)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }

    internal class AccessGroupsUpdatedEventArgs : EventArgs
    {
        internal IEnumerable<AccessGroup> AccessGroups { get; set; }
        internal bool ShouldUpdateClients { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessGroups"></param>
        /// <param name="shouldUpdateClients">False if only users are added, modified or deleted</param>
        internal AccessGroupsUpdatedEventArgs(IEnumerable<AccessGroup> accessGroups, bool shouldUpdateClients)
        {
            AccessGroups = accessGroups;
            ShouldUpdateClients = shouldUpdateClients;
        }
    }
}
