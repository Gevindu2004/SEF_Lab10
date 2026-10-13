import { useState } from "react";
import { useDebounce } from "../hooks/useDebounce";
import { useFetch } from "../hooks/useFetch";
import MenuList from "../components/MenuList";

function MenuPage() {
  const [query, setQuery] = useState("");
  const debounced = useDebounce(query, 400);

  // Frontend: read the API base URL from the environment (Task 01)
  const apiBase = import.meta.env.VITE_API_URL;
  const menuEndpoint = apiBase ? `${apiBase.replace(/\/$/, "")}/api/menu` : "/menu.json";
  const { data: dishes, isLoading, error } = useFetch(menuEndpoint);

  if (isLoading) return <p>Loading menu...</p>;
  if (error) return <p>Could not load menu: {error}</p>;

  const menuItems = Array.isArray(dishes) ? dishes : [];
  const filtered = menuItems.filter((d) =>
    d.name.toLowerCase().includes(debounced.toLowerCase())
  );

  return (
    <div>
      <input
        value={query}
        onChange={(e) => setQuery(e.target.value)}
        placeholder="Search dishes..."
      />
      <MenuList dishes={filtered} />
    </div>
  );
}

export default MenuPage;
