using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // Tambahkan ini agar bisa akses Session
using BlockChain.Models;
using System;
using System.Collections.Generic;

namespace BlockChain.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Chat(int userId)
        {
            // Simulasi ambil role user login (misal dari session)
            var role = HttpContext.Session.GetString("Role"); // Contoh: "Gudang", "Owner", "Keuangan"

            // Simulasi daftar pesan berdasarkan userId
            var pesan = GetDummyMessages(userId);

            // Redirect ke tampilan berdasarkan role
            if (role == "Gudang")
            {
                return View("ChatGudang", pesan);
            }
            else if (role == "Owner")
            {
                return View("Chat", pesan);
            }
            else if (role == "Keuangan")
            {
                return View("ChatKeuangan", pesan);
            }

            // Jika role tidak dikenali, fallback ke tampilan default (Gudang)
            return View("ChatGudang", pesan);
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
