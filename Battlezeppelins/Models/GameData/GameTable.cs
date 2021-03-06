﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Battlezeppelins.Models
{

    public class GameTable
    {
        public const int TABLE_ROWS = 10;
        public const int TABLE_COLS = 10;

        [JsonProperty]
        public Game.Role role { get; private set; }
        [JsonProperty]
        public List<OpenPoint> openPoints { get; private set; }
        [JsonProperty]
        public List<Zeppelin> zeppelins { get; private set; }

        private GameTable() { }

        public GameTable(Game.Role role)
        {
            this.role = role;

            this.openPoints = new List<OpenPoint>();
            this.zeppelins = new List<Zeppelin>();
        }

        public static GameTable deserialize(string serialized)
        {
            var settings = new JsonSerializerSettings
            {
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            };
            GameTable deserialized = JsonConvert.DeserializeObject<GameTable>(serialized, settings);

            return deserialized;
        }

        public string serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public bool AddZeppelin(Zeppelin newZeppelin)
        {
            foreach (Zeppelin zeppelin in this.zeppelins)
            {
                // No multiple zeppelins of the same type
                if (zeppelin.type == newZeppelin.type)
                    return false;

                // No colliding zeppelins
                if (zeppelin.collides(newZeppelin))
                    return false;
            }

            int x = newZeppelin.location.x;
            int y = newZeppelin.location.y;
            int w = newZeppelin.getWidth();
            int h = newZeppelin.getHeight();
            // Zeppelin must be in the table
            if (x < 0 || x + w -1 > TABLE_COLS - 1 ||
                y < 0 || y + h -1 > TABLE_ROWS - 1)
            {
                return false;
            }

            this.zeppelins.Add(newZeppelin);

            return true;
        }

        /// <summary>
        /// Remove all zeppelins that have not been found
        /// </summary>
        /// <param name="openPoints"></param>
        public void removeNonOpenZeppelins() {
            foreach (Zeppelin zeppelin in zeppelins.ToList())
            {
                List<Point> points = new List<Point>(this.openPoints);
                if (!(zeppelin.fullyCollides(points)))
                {
                    zeppelins.Remove(zeppelin);
                }
            }
        }

				public void removeZeppelins()
				{
					foreach (Zeppelin zeppelin in zeppelins.ToList())
					{
						List<Point> points = new List<Point>(this.openPoints);
							zeppelins.Remove(zeppelin);
					}
				}

        public bool pointCollides(Point point)
        {
            foreach (Zeppelin zeppelin in zeppelins)
            {
                if (zeppelin.collides(point)) return true;
            }
            return false;
        }

        public bool alreadyOpen(Point point)
        {
            foreach (OpenPoint openPoint in openPoints) {
                if (point.Equals((Point)openPoint)) return true;
            }
            return false;
        }
    }
}
