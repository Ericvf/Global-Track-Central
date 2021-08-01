using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Trials.GTC.Mobile.Extensions
{
    public class IntType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public static class IntExtensions
    {
        static List<IntType> trialsVersions = new List<IntType>()
        {
            new IntType() { Id = 0, Name = @"Trials 2"},
            new IntType() { Id = 1, Name = @"Trials HD"},
            new IntType() { Id = 2, Name = @"Evolution X360"},
            new IntType() { Id = 3, Name = @"Evolution PC"},
            new IntType() { Id = 4, Name = @"Fusion PC"},
            new IntType() { Id = 5, Name = @"Fusion 360"},
            new IntType() { Id = 6, Name = @"Fusion XB1"},
            new IntType() { Id = 7, Name = @"Fusion PS4"},
        };

        static List<IntType> trialsTypes = new List<IntType>()
        {
            new IntType() { Id = 0, Name = @"Trials"},
            new IntType() { Id = 1, Name = @"Supercross"},
            new IntType() { Id = 2, Name = @"Skillgame"},
            new IntType() { Id = 3, Name = @"FMX"},
        };

        static List<IntType> difficulties = new List<IntType>()
        {
            new IntType() { Id = 0, Name = @"Beginner"},
            new IntType() { Id = 1, Name = @"Easy"},
            new IntType() { Id = 2, Name = @"Medium"},
            new IntType() { Id = 3, Name = @"Hard"},
            new IntType() { Id = 4, Name = @"Extreme"},
            new IntType() { Id = 5, Name = @"Ninja 1"},
            new IntType() { Id = 6, Name = @"Ninja 2"},
            new IntType() { Id = 7, Name = @"Ninja 3"},
            new IntType() { Id = 8, Name = @"Ninja 4"},
            new IntType() { Id = 9, Name = @"Ninja 5"},
        };

        public static string ToTrialsVersion(this int v)
        {
            var result = trialsVersions.FirstOrDefault(it => it.Id.Equals(v));
            if (result != null)
                return result.Name;

            return null;
        }

        public static string ToTrialsType(this int v)
        {
            var result = trialsTypes.FirstOrDefault(it => it.Id.Equals(v));
            if (result != null)
                return result.Name;

            return null;
        }
        public static string ToDifficulty(this int v)
        {
            var result = difficulties.FirstOrDefault(it => it.Id.Equals(v));
            if (result != null)
                return result.Name;

            return null;
        }
    }
}