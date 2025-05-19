// WhatsMissingService.cs
using Lek8LarBackend.Models.MemoryGameModels;

namespace Lek8LarBackend.Services.MemoryGames
{
    public class WhatsMissingService
    {
        private readonly Random _random = new();

        private readonly string[] _imagePool = new[]
        {
            "katt.png", "hund.png", "våffla.png", "munk.png", "bulle.png", "juicebox.png"
        };

        public WhatsMissingQuestion GenerateQuestion()
        {
            var selected = _imagePool.OrderBy(_ => _random.Next()).Take(4).ToList();
            var missing = selected[_random.Next(selected.Count)];
            var remaining = selected.Where(i => i != missing).ToList();

            var options = new HashSet<string> { missing };
            while (options.Count < 3)
            {
                var opt = _imagePool[_random.Next(_imagePool.Length)];
                options.Add(opt);
            }

            return new WhatsMissingQuestion
            {
                AllImages = selected,
                RemainingImages = remaining,
                CorrectAnswer = missing,
                Options = options.OrderBy(_ => _random.Next()).ToList()
            };
        }
    }
}
