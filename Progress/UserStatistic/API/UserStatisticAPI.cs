using Entity;
using Entity.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Configuration;
using System.Data;
using View.Models.In;
using View.Models.Out;

namespace UserStatistic.API
{
    //Класс реализует методы для вызова из контролерров
    //В нем реализованна логика обработки данных
    //и соединение с БД через DatabaseContext и NpgsqlConnection
    //где для выполнения задач не подходит DatabaseContext
    //используеться NpgsqlConnection, как вспомогательное подключение
    public class UserStatisticAPI
    {
        private readonly DatabaseContext _context;
        //строка соединения во всем сервисе для разных подключений одна и та-же и тянеться из настроек
        public static string _connstr;
        NpgsqlConnection _npgconn = new NpgsqlConnection(_connstr);
        public UserStatisticAPI(DatabaseContext context) 
        {
            _context = context;
        }

        private OutUserStatistics GetOutUserStatistics(object[] userStatistic)
        {
            OutUserStatistics result = new OutUserStatistics
            {
                UserId = Convert.ToInt64(userStatistic[1]),
                Level = (Level)userStatistic[2],
                Rating = Convert.ToInt32(userStatistic[3]),
                IsPrivileged = (bool)userStatistic[4],
                GameCount = Convert.ToInt32(userStatistic[5]),
                WinGames = Convert.ToInt32(userStatistic[6]),
                LossGames = Convert.ToInt32(userStatistic[7])
            };

            return result;
        }

        //метод для обновления рейтинга и игр игрока по его ID
        //На повышение рейтинга влияют два параметра:
        //GameCount количество сыгранных игр = 3
        //WinGames количество выиграных игр = 2
        public async Task<bool> UpdateRatingGameAsync(long userid, int raiting = 0, int gamecount = 0, int gameW = 0, int gameL = 0)
        {
            double procentLvl = 0;
            var user = _context.UserStatistics.FirstOrDefault(f => f.UserId == userid);
            if (user == null)
                return false;

            int currentRaiting = raiting == 0 ? user.Rating : raiting;

            if(raiting > 0)
                user.Rating = currentRaiting;
            if (gamecount > 0)
                user.GameCount += gamecount;
            if (gameW > 0)
                user.WinGames += gameW;
            if (gameL > 0)
                user.LossGames += gameL;

            procentLvl = Math.Round((double)(user.WinGames * 100 / user.GameCount) - (double)(100/currentRaiting));
            if (procentLvl > 10)
            {
                user.Level = GetLevel(procentLvl, user.IsPrivileged);
            }

            _context.SaveChangesAsync();

            return true; 
        }

        //Метод выставляет уровень играка на основе процентного соотношения выигранных и проигранных игр
        //в зависимости превилигированного статуса
        private Level GetLevel(double lvl, bool isPrvl)
        {
            Level result = new Level();
            if (isPrvl)
            {
                if (lvl > 10 && lvl < 15)
                {
                    result = Level.Elementary;
                }
                else if (lvl > 15 && lvl < 20)
                {
                    result = Level.New;
                }
                else if (lvl > 20 && lvl < 30)
                {
                    result = Level.Middle;
                }
                else if (lvl > 30 && lvl < 45)
                {
                    result = Level.Professional;
                }
                else if (lvl > 45 && lvl < 65)
                {
                    result = Level.Expret;
                }
                else if (lvl > 65)
                {
                    result = Level.WorldClass;
                }
            }
            else
            {
                if (lvl > 20 && lvl < 30)
                {
                    result = Level.Elementary;
                }
                else if (lvl > 30 && lvl < 40)
                {
                    result = Level.New;
                }
                else if (lvl > 40 && lvl < 55)
                {
                    result = Level.Middle;
                }
                else if (lvl > 55 && lvl < 70)
                {
                    result = Level.Professional;
                }
                else if (lvl > 70 && lvl < 90)
                {
                    result = Level.Expret;
                }
                else if (lvl > 90)
                {
                    result = Level.WorldClass;
                }
            }

            return result;
        }

        //метод для получений игроков с наибольшим рейтингом
        public List<OutUserStatistics> GetTop(int count)
        {
            DataSet ds = new DataSet();
            List<OutUserStatistics> result = new List<OutUserStatistics>();
            _npgconn.Open();

            string sql = "SELECT * FROM \"user\".\"UserStatistic\" order by \"Rating\" desc";

            using (NpgsqlCommand command = new NpgsqlCommand(sql, _npgconn))
            {
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);
                da.Fill(ds);

                _npgconn.Close();
            }

            if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
                return null;

            foreach (DataRow rw in ds.Tables[0].Rows)
            {
                result.Add(GetOutUserStatistics(rw.ItemArray));
            }

            return result;
        }

        //метод для получения статистики игрока по его ID
        public OutUserStatistics Get(long userid)
        {
            var userStatistic = _context.UserStatistics.FirstOrDefault(f => f.UserId == userid);

            if (userStatistic == null)
                return null;

            OutUserStatistics result = new OutUserStatistics
            {
                UserId = userStatistic.UserId,
                Level = userStatistic.Level,
                Rating = userStatistic.Rating,
                IsPrivileged = userStatistic.IsPrivileged,
                GameCount = userStatistic.GameCount,
                WinGames = userStatistic.WinGames,
                LossGames = userStatistic.LossGames
            };

            return result;
        }

        //метод меняет статус превелегированного пользователя на указанный по ID пользователя
        public bool UpdatePrivileged(long userid, bool privileg)
        {
            var user = _context.UserStatistics.FirstOrDefault(f => f.UserId == userid);
            if (user == null)
                return false;

            user.IsPrivileged = privileg;

            _context.SaveChangesAsync();

            return true;
        }

        //метод обновляет статистику по пользователю должен вызываться автоматически после завершения каждой игры
        public bool UpdateUserStatistic(InUserStatistics userStatistics)
        {
            bool result = true;
            Task<bool> task = null;
            long userID = userStatistics.UserId;
            int gameCount, wGames;
            var user = _context.UserStatistics.FirstOrDefault(f => f.UserId == userID);
            if(user == null)
                return false;

            gameCount = user.GameCount + userStatistics.GameCount;
            wGames = user.WinGames + userStatistics.WinGames;

            if (gameCount % 3 == 0 || wGames % 2 == 0)
            {
                if (gameCount % 3 == 0 && wGames % 2 == 0)
                {
                    int rating = (gameCount / 3) + (wGames / 2);
                    task = UpdateRatingGameAsync(userID, rating, userStatistics.GameCount, userStatistics.WinGames, userStatistics.LossGames);
                    if (!task.Result)
                        result = false;
                }
                else if (gameCount % 3 == 0)
                {
                    int rating = (gameCount / 3) + (int)Math.Truncate((double)(user.WinGames / 2));
                    task = UpdateRatingGameAsync(userID, rating, userStatistics.GameCount, userStatistics.WinGames, userStatistics.LossGames);
                    if (!task.Result)
                        result = false;
                }
                else if (wGames % 2 == 0)
                {
                    int rating = (int)Math.Truncate((double)(user.GameCount / 3)) + (wGames / 2);
                    task = UpdateRatingGameAsync(userID, rating, userStatistics.GameCount, userStatistics.WinGames, userStatistics.LossGames);
                    if (!task.Result)
                        result = false;
                }
            }

            return result;
        }
        //метод для создания новой статистики по имеющимуся игроку
        //проверка существования игрока проверяеться по iD в схеме - auth в таблице - users
        //если запись по игроку с указанным ID уже существует в БД вызываеться метод обновления статистики игрока
        public bool CreateUserStatistic(InUserStatistics userStatistics)
        {
            bool result = true;
            long userCheck = 0;
            long userID = userStatistics.UserId;

            _npgconn.Open();

            string sql = "SELECT count(*) FROM \"auth\".\"users\" where \"id\" = " + userID.ToString();
            string sqlCheck = "SELECT count(*) FROM \"user\".\"UserStatistic\" where \"UserId\" = " + userID.ToString();

            using (NpgsqlCommand command = new NpgsqlCommand(sql, _npgconn))
            {
                if (Convert.ToInt64(command.ExecuteScalar()) > 0)
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand(sqlCheck, _npgconn))
                    {
                        userCheck = Convert.ToInt64(cmd.ExecuteScalar());
                    }
                }
                else
                {
                    result = false;
                }

                _npgconn.Close();
            }

            if (result && userCheck == 0)
            {
                _npgconn.Open();

                sql = "INSERT INTO \"user\".\"UserStatistic\"(\"UserId\", \"Level\", \"Rating\", \"IsPrivileged\", \"GameCount\", \"WinGames\", \"LossGames\") VALUES (" + userID.ToString() + ", 0, 0, false, " + userStatistics.GameCount.ToString() + ", " + userStatistics.WinGames.ToString() + ", " + userStatistics.LossGames.ToString() + ");";

                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, _npgconn))
                {
                    if (cmd.ExecuteNonQuery() > 0)
                        result = true;
                }

                _npgconn.Close();
            }
            else if(result && userCheck > 0)
            {
                UpdateUserStatistic(userStatistics);
            }

            return result;
        }
    }
}
