﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models
{
    [Table("UserStatistic", Schema = "user")]
    public class UserStatistic
    {
        [Column("id"), Required]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column("userid"), Required]
        public long UserId { get; set; }

        [Column("level"), Required]
        public Level Level { get; set; }

        [Column("rating")]
        public int Rating { get; set; }

        [Column("isprivileged")]
        public bool IsPrivileged { get; set; }

        [Column("gamecount")]
        public int GameCount { get; set; }

        /// <summary>
        /// TODO: По оканчание игры нужно перехватывать событие от сервиса GameStatistics и обновлять данные значения
        /// </summary>
        [Column("wingames"), Required]
        public int WinGames { get; set; }

        [Column("lossgames"), Required]
        public int LossGames { get; set; }
    }

    public enum Level
    {
        Elementary,
        New,
        Middle,
        Professional,
        Expret,
        WorldClass
    }
}
