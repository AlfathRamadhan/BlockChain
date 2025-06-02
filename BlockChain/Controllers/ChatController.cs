using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BlockChain.Models;
using System;
using System.Collections.Generic;

namespace BlockChain.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Chat(int userId)
        {
            // Ambil role dari session
            var role = HttpContext.Session.GetString("Role");

            // Jika session tidak ada atau role kosong, redirect ke login
            if (string.IsNullOrEmpty(role))
            {
                return RedirectToAction("Login", "Account");
            }

            // Simulasi daftar pesan berdasarkan userId
            var pesan = GetDummyMessages(userId);

            // Routing ke view berdasarkan role
            switch (role)
            {
                case "Gudang":
                    return View("ChatGudang", pesan);
                case "Owner":
                    return View("Chat", pesan);
                case "Keuangan":
                    return View("ChatKeuangan", pesan);
                case "Distributor":
                    return View("ChatDistributor", pesan);
                default:
                    // Jika role tidak dikenali, redirect ke login
                    return RedirectToAction("Login", "Account");
            }
        }

        private List<ChatMessage> GetDummyMessages(int userId)
        {
            if (userId == 1)
            {
                return new List<ChatMessage>
                {
                    new ChatMessage { Pengirim = "PT. Blalala", Penerima = "Me", Pesan = "Halo dari Owner", Tanggal = DateTime.Now.AddMinutes(-10), IsMe = false },
                    new ChatMessage { Pengirim = "Me", Penerima = "PT. Blalala", Pesan = "Siap!", Tanggal = DateTime.Now.AddMinutes(-5), IsMe = true }
                };
            }
            else if (userId == 2)
            {
                return new List<ChatMessage>
                {
                    new ChatMessage { Pengirim = "PT. Blulala", Penerima = "Me", Pesan = "Selamat pagi Keuangan!", Tanggal = DateTime.Now.AddHours(-1), IsMe = false },
                    new ChatMessage { Pengirim = "Me", Penerima = "PT. Blulala", Pesan = "Pagi!", Tanggal = DateTime.Now.AddMinutes(-30), IsMe = true }
                };
            }
            else
            {
                return new List<ChatMessage>
                {
                    new ChatMessage { Pengirim = "PT. Generic", Penerima = "Me", Pesan = "Ini pesan default.", Tanggal = DateTime.Now, IsMe = false }
                };
            }
        }
    }
}
