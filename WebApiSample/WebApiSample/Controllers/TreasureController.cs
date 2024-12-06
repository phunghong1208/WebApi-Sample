using System;
using System.Collections.Generic;
using System.Web.Mvc;
using WebApiSample.Models;
using Newtonsoft.Json;

namespace WebApiSample.Controllers
{
    public class TreasureController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TreasureController()
        {
            _context = new ApplicationDbContext();
        }

        // Action để hiển thị trang nhập bản đồ kho báu
        public ActionResult Index()
        {
            return View();
        }

        // Action để tính toán lượng nhiên liệu ít nhất
        [HttpPost]
        public ActionResult CalculateFuel(TreasureMapInput input)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid input" });
            }

            double fuel = CalculateMinimumFuel(input);
            return Json(new { success = true, fuel });
        }

        // Hàm tính toán nhiên liệu tối thiểu (Sử dụng thuật toán BFS)
        private double CalculateMinimumFuel(TreasureMapInput input)
        {
            int n = input.N;
            int m = input.M;
            int[,] grid = input.Grid;
            int target = input.P;

            // Tìm vị trí chứa rương có số thứ tự là p
            (int targetX, int targetY) = FindTreasureLocation(grid, target, n, m);

            // Sử dụng BFS để tìm đường đi từ (0, 0) đến vị trí chứa rương
            return BfsMinimumFuel(grid, n, m, targetX, targetY);
        }

        // Hàm tìm vị trí chứa rương thứ p
        private (int, int) FindTreasureLocation(int[,] grid, int target, int n, int m)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    if (grid[i, j] == target)
                    {
                        return (i, j);
                    }
                }
            }
            return (-1, -1);  // Trả về (-1, -1) nếu không tìm thấy (không bao giờ xảy ra trong bài toán này)
        }

        // Hàm BFS để tính toán nhiên liệu tối thiểu
        private double BfsMinimumFuel(int[,] grid, int n, int m, int targetX, int targetY)
        {
            // 4 hướng di chuyển: lên, xuống, trái, phải
            int[] dx = { -1, 1, 0, 0 };
            int[] dy = { 0, 0, -1, 1 };

            // Tạo mảng visited để kiểm tra xem ô đã được thăm chưa
            bool[,] visited = new bool[n, m];
            Queue<(int, int, int)> queue = new Queue<(int, int, int)>(); // (x, y, bước đi)
            queue.Enqueue((0, 0, 0)); // Bắt đầu từ (0, 0) và bước đi = 0
            visited[0, 0] = true;

            // BFS
            while (queue.Count > 0)
            {
                var (x, y, step) = queue.Dequeue();

                // Nếu tìm thấy rương, trả về số bước đi
                if (x == targetX && y == targetY)
                {
                    return step; // Lượng nhiên liệu là số bước di chuyển
                }

                // Thăm các ô lân cận
                for (int i = 0; i < 4; i++)
                {
                    int nx = x + dx[i];
                    int ny = y + dy[i];

                    // Kiểm tra nếu ô (nx, ny) hợp lệ và chưa thăm
                    if (nx >= 0 && nx < n && ny >= 0 && ny < m && !visited[nx, ny])
                    {
                        visited[nx, ny] = true;
                        queue.Enqueue((nx, ny, step + 1)); // Thêm ô mới vào queue và tăng bước đi lên 1
                    }
                }
            }

            return -1; // Trả về -1 nếu không tìm thấy (không xảy ra trong bài toán này)
        }

        // Action lưu trữ bản đồ kho báu vào cơ sở dữ liệu
        [HttpPost]
        public ActionResult SaveTreasureMap(TreasureMap input)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid input" });
            }

            var treasureMap = new TreasureMap
            {
                N = input.N,
                M = input.M,
                P = input.P,
                Grid = JsonConvert.SerializeObject(input.Grid),
                CreatedAt = DateTime.Now
            };

            _context.TreasureMaps.Add(treasureMap);
            _context.SaveChanges();

            return Json(new { success = true, message = "Treasure map saved successfully!" });
        }
    }
}
