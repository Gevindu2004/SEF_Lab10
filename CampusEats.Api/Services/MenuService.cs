using CampusEats.Api.Data;
using CampusEats.Api.Dtos;
using CampusEats.Api.Models;

namespace CampusEats.Api.Services;

public class MenuService : IMenuService
{
    private readonly CampusEatsDbContext _db;
    private static readonly List<MenuItem> _fallbackItems = new()
    {
        new() { Id = 1, Name = "Kottu Roti", Price = 750m, Category = "Mains", Available = true },
        new() { Id = 2, Name = "Fried Rice", Price = 850m, Category = "Mains", Available = true },
        new() { Id = 3, Name = "Veg Sub", Price = 600m, Category = "Snacks", Available = false },
        new() { Id = 4, Name = "Watalappan", Price = 350m, Category = "Dessert", Available = true },
        new() { Id = 5, Name = "Iced Milo", Price = 300m, Category = "Drinks", Available = true },
    };
    private static int _nextId = 6;

    public MenuService(CampusEatsDbContext db)
    {
        _db = db;
    }

    private static MenuItemDto ToDto(MenuItem m) =>
        new(m.Id, m.Name, m.Price, m.Category, m.Available);

    private bool IsDbAvailable()
    {
        try
        {
            return _db.Database.CanConnect();
        }
        catch
        {
            return false;
        }
    }

    public IEnumerable<MenuItemDto> GetAll()
    {
        if (IsDbAvailable())
        {
            try
            {
                return _db.MenuItems.OrderBy(x => x.Id).Select(ToDto).ToList();
            }
            catch { }
        }
        return _fallbackItems.Select(ToDto).ToList();
    }

    public MenuItemDto? GetById(int id)
    {
        if (IsDbAvailable())
        {
            try
            {
                var m = _db.MenuItems.Find(id);
                return m is null ? null : ToDto(m);
            }
            catch { }
        }
        var fallback = _fallbackItems.FirstOrDefault(x => x.Id == id);
        return fallback is null ? null : ToDto(fallback);
    }

    public MenuItemDto Create(CreateMenuItemDto dto)
    {
        var item = new MenuItem
        {
            Name = dto.Name,
            Price = dto.Price,
            Category = dto.Category,
            Available = true
        };

        if (IsDbAvailable())
        {
            try
            {
                _db.MenuItems.Add(item);
                _db.SaveChanges();
                return ToDto(item);
            }
            catch { }
        }

        item.Id = _nextId++;
        _fallbackItems.Add(item);
        return ToDto(item);
    }

    public bool Update(int id, CreateMenuItemDto dto)
    {
        if (IsDbAvailable())
        {
            try
            {
                var m = _db.MenuItems.Find(id);
                if (m is null) return false;
                m.Name = dto.Name;
                m.Price = dto.Price;
                m.Category = dto.Category;
                _db.SaveChanges();
                return true;
            }
            catch { }
        }

        var f = _fallbackItems.FirstOrDefault(x => x.Id == id);
        if (f is null) return false;
        f.Name = dto.Name;
        f.Price = dto.Price;
        f.Category = dto.Category;
        return true;
    }

    public bool Delete(int id)
    {
        if (IsDbAvailable())
        {
            try
            {
                var m = _db.MenuItems.Find(id);
                if (m is null) return false;
                _db.MenuItems.Remove(m);
                _db.SaveChanges();
                return true;
            }
            catch { }
        }

        var f = _fallbackItems.FirstOrDefault(x => x.Id == id);
        if (f is null) return false;
        _fallbackItems.Remove(f);
        return true;
    }
}