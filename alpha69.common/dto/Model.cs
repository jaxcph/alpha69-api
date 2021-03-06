﻿using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace alpha69.common.dto
{
    public class Model
    {
        protected DateTime _createdAt;


        protected Product[] _products;


        public Model()
        {
        }

        public Model(DataRow row)
        {
            Id = Convert.ToInt32(row["id"]);
            _createdAt = Convert.ToDateTime(row["created_at"]);
            UserId = Convert.ToInt32(row["user_id"]);
            Name = row["name"] as string;
            Description = row["description"] as string;
            Website = row["website"] as string;
            Facebook = row["facebook"] as string;
            Twitter = row["twitter"] as string;
            Instagram = row["instagram"] as string;
            Snapchat = row["snapchat"] as string;
        }

        public int Id { get; private set; }
        public DateTime CreatedAt => _createdAt;


        public int UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Website { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string Snapchat { get; set; }

        public Product[] Products => _products;


        public static Model Load(int id, bool includeProducts, MySqlConnection conn)
        {
            var da = new MySqlDataAdapter(
                $"SELECT id,user_id,name,description,website,facebook,twitter,instagram,snapchat, created_at FROM models WHERE id={id}",
                conn);
            var ds = new DataSet("models");
            da.Fill(ds);

            if (ds.Tables[0].Rows.Count == 1)
            {
                var item = new Model(ds.Tables[0].Rows[0]);
                item._products = Product.LoadForModel(id, conn).ToArray();
                return item;
            }

            return null;
        }

        public static Model LoadByUser(int userId, bool includeProducts, MySqlConnection conn)
        {
            var da = new MySqlDataAdapter(
                $"SELECT id,user_id,name,description,website,facebook,twitter,instagram,snapchat, created_at FROM models WHERE user_id={userId}",
                conn);
            var ds = new DataSet("models");
            da.Fill(ds);

            if (ds.Tables[0].Rows.Count == 1)
            {
                var item = new Model(ds.Tables[0].Rows[0]);
                if (includeProducts)
                    item._products = Product.LoadForModel(item.Id, conn).ToArray();

                return item;
            }

            return null;
        }


        public static Model LoadByName(string name, bool includeProducts, MySqlConnection conn)
        {
            var da = new MySqlDataAdapter(
                $"SELECT id,user_id,name,description,website,facebook,twitter,instagram,snapchat, created_at FROM models WHERE name=@name",
                conn);
            da.SelectCommand.Parameters.Add("@name", MySqlDbType.VarChar);
            da.SelectCommand.Parameters[0].Value = name;

            var ds = new DataSet("models");
            da.Fill(ds);

            if (ds.Tables[0].Rows.Count == 1)
            {
                var item = new Model(ds.Tables[0].Rows[0]);
                if (includeProducts)
                    item._products = Product.LoadForModel(item.Id, conn).ToArray();

                return item;
            }

            return null;
        }


        public void Save(MySqlConnection conn)
        {
            var cmd = new MySqlCommand(
                $"INSERT INTO models(user_id,name,description,website,facebook,twitter,instagram,snapchat) VALUES ({UserId}, @name, @description, @website, @facebook, @twitter, @instagram, @snapschat);COMMIT;",
                conn);
            cmd.Parameters.Add("@name", MySqlDbType.VarChar);
            cmd.Parameters.Add("@description", MySqlDbType.VarChar);
            cmd.Parameters.Add("@website", MySqlDbType.VarChar);
            cmd.Parameters.Add("@facebook", MySqlDbType.VarChar);
            cmd.Parameters.Add("@twitter", MySqlDbType.VarChar);
            cmd.Parameters.Add("@instagram", MySqlDbType.VarChar);
            cmd.Parameters.Add("@snapschat", MySqlDbType.VarChar);

            cmd.Parameters["@name"].Value = Name;

            if (string.IsNullOrEmpty(Description))
                cmd.Parameters["@description"].Value = DBNull.Value;
            else
                cmd.Parameters["@description"].Value = Description;

            if (string.IsNullOrEmpty(Website))
                cmd.Parameters["@website"].Value = DBNull.Value;
            else
                cmd.Parameters["@website"].Value = Website;

            if (string.IsNullOrEmpty(Facebook))
                cmd.Parameters["@facebook"].Value = DBNull.Value;
            else
                cmd.Parameters["@facebook"].Value = Facebook;

            if (string.IsNullOrEmpty(Twitter))
                cmd.Parameters["@twitter"].Value = DBNull.Value;
            else
                cmd.Parameters["@twitter"].Value = Twitter;

            if (string.IsNullOrEmpty(Instagram))
                cmd.Parameters["@instagram"].Value = DBNull.Value;
            else
                cmd.Parameters["@instagram"].Value = Instagram;

            if (string.IsNullOrEmpty(Snapchat))
                cmd.Parameters["@snapschat"].Value = DBNull.Value;
            else
                cmd.Parameters["@snapschat"].Value = Snapchat;


            var cmdSelect = new MySqlCommand($"SELECT id FROM models WHERE user_id={UserId}", conn);

            conn.Open();
            cmd.ExecuteNonQuery();
            var o = cmdSelect.ExecuteScalar();
            Id = Convert.ToInt32(o);
            conn.Close();
        }

        public void Update(MySqlConnection conn)
        {
            var cmd = new MySqlCommand(
                $"UPDATE models SET name=@name, description=@description, website=@website, facebook=@facebook, twitter=@twitter, instagram=@instagram, snapchat=@snapschat WHERE id={Id};COMMIT;",
                conn);
            cmd.Parameters.Add("@name", MySqlDbType.VarChar);
            cmd.Parameters.Add("@description", MySqlDbType.VarChar);
            cmd.Parameters.Add("@website", MySqlDbType.VarChar);
            cmd.Parameters.Add("@facebook", MySqlDbType.VarChar);
            cmd.Parameters.Add("@twitter", MySqlDbType.VarChar);
            cmd.Parameters.Add("@instagram", MySqlDbType.VarChar);
            cmd.Parameters.Add("@snapschat", MySqlDbType.VarChar);

            cmd.Parameters["@name"].Value = Name;

            if (string.IsNullOrEmpty(Description))
                cmd.Parameters["@description"].Value = DBNull.Value;
            else
                cmd.Parameters["@description"].Value = Description;

            if (string.IsNullOrEmpty(Website))
                cmd.Parameters["@website"].Value = DBNull.Value;
            else
                cmd.Parameters["@website"].Value = Website;

            if (string.IsNullOrEmpty(Facebook))
                cmd.Parameters["@facebook"].Value = DBNull.Value;
            else
                cmd.Parameters["@facebook"].Value = Facebook;

            if (string.IsNullOrEmpty(Twitter))
                cmd.Parameters["@twitter"].Value = DBNull.Value;
            else
                cmd.Parameters["@twitter"].Value = Twitter;

            if (string.IsNullOrEmpty(Instagram))
                cmd.Parameters["@instagram"].Value = DBNull.Value;
            else
                cmd.Parameters["@instagram"].Value = Instagram;

            if (string.IsNullOrEmpty(Snapchat))
                cmd.Parameters["@snapschat"].Value = DBNull.Value;
            else
                cmd.Parameters["@snapschat"].Value = Snapchat;

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }
    }
}