﻿using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace alpha69.common.dto
{
    public class Goal
    {
        protected DateTime _abortedAt;

        protected DateTime _completedAt;


        protected DateTime _createdAt;


        public Goal()
        {
        }

        public Goal(DataRow row)
        {
            Id = Convert.ToInt32(row["id"]);
            _createdAt = Convert.ToDateTime(row["created_at"]);


            if (!row.IsNull("aborted_at"))
                _abortedAt = Convert.ToDateTime(row["aborted_at"]);

            if (!row.IsNull("completed_at"))
                _completedAt = Convert.ToDateTime(row["completed_at"]);

            LiveSessionId = Convert.ToInt32(row["live_session_id"]);
            GoalAmount = Convert.ToDouble(row["goal_amount"]);
            GoalAmountLeft = Convert.ToDouble(row["goal_left"]);
            ProductId = Convert.ToInt32(row["product_id"]);

            Title = row["title"] as string;
            Description = row["description"] as string;
            Tags = row["tags"] as string;
        }

        public int Id { get; private set; }
        public DateTime CreatedAt => _createdAt;
        public DateTime CompletedAt => _completedAt;
        public DateTime AbortedAt => _abortedAt;

        public int LiveSessionId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }
        public int ProductId { get; set; }
        public double GoalAmount { get; set; }
        public double GoalAmountLeft { get; set; }

        public static List<Goal> LoadOpenByLiveSession(int livesessionId, MySqlConnection conn)
        {
            var list = new List<Goal>();

            var da = new MySqlDataAdapter(
                $"SELECT id, live_session_id, title, description, goal_amount, goal_left, created_at, completed_at, aborted_at, tags,  product_id  FROM live_session_goals WHERE (live_session_id={livesessionId}) AND ( completed_at IS NULL) AND (aborted_at IS NULL) ORDER BY id DESC",
                conn);
            var ds = new DataSet("live_session_goals");
            da.Fill(ds);

            if (ds.Tables[0].Rows.Count > 0)
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var item = new Goal(row);
                    list.Add(item);
                }

            return list;
        }

        public static void SetAmountLeft(int id, double amount, MySqlConnection conn)
        {
            var cmd = new MySqlCommand($"UPDATE live_session_goals SET amount_left={amount} WHERE (id={id})", conn);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }


        public static void SetCompleted(int id, MySqlConnection conn)
        {
            var cmd = new MySqlCommand($"UPDATE live_session_goals SET completed_at=now() WHERE (id={id})", conn);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public static void Abort(int id, MySqlConnection conn)
        {
            var cmd = new MySqlCommand($"UPDATE live_session_goals SET aborted_at=now() WHERE (id={id})", conn);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public void Save(MySqlConnection conn)
        {
            //abort exisiting open of any
            var cmdExisting =
                new MySqlCommand(
                    $"UPDATE live_session_goals SET aborted_at=now() WHERE (live_session_id={LiveSessionId} AND product_id={ProductId}) AND (completed_at IS NULL AND aborted_at IS NULL)",
                    conn);


            //insert new
            var cmdInsert = new MySqlCommand(
                $"INSERT INTO live_session_goals(live_session_id, title, description, goal_amount, goal_left, tags, product_id) VALUES({LiveSessionId},@title,@description,{GoalAmount},{GoalAmountLeft},@tags,{ProductId});COMMIT;SELECT LAST_INSERT_ID()",
                conn);

            cmdInsert.Parameters.Add("@title", MySqlDbType.VarChar);
            cmdInsert.Parameters.Add("@description", MySqlDbType.VarChar);
            cmdInsert.Parameters.Add("@tags", MySqlDbType.VarChar);

            cmdInsert.Parameters["@title"].Value = Title;

            if (string.IsNullOrEmpty(Description))
                cmdInsert.Parameters["@description"].Value = DBNull.Value;
            else
                cmdInsert.Parameters["@description"].Value = Description;

            if (string.IsNullOrEmpty(Tags))
                cmdInsert.Parameters["@tags"].Value = DBNull.Value;
            else
                cmdInsert.Parameters["@tags"].Value = Tags;

            conn.Open();
            cmdExisting.ExecuteNonQuery(); //closes abandoned goals
            var o = cmdInsert.ExecuteScalar(); //insert a new goal
            Id = Convert.ToInt32(o);
            conn.Close();
        }
    }
}