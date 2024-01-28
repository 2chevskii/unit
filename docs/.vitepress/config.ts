import { defineConfig } from "vitepress";
import nugetIcon from "./resources/nuget-icon.js";

// https://vitepress.dev/reference/site-config
export default defineConfig({
  title: "Dvchevskii.Unit",
  description: "Dvchevskii.Unit package documentation",
  appearance: "force-dark",
  themeConfig: {
    // https://vitepress.dev/reference/default-theme-config
    nav: [
      { text: "Home", link: "/" },
      { text: "Guide", link: "/guide/installation" },
      { text: "API", link: "/api/" },
    ],

    sidebar: [
      {
        text: "Guide",
        items: [
          {
            text: "Installation",
            link: "/guide/installation",
          },
          {
            text: "Usage",
            link: "/guide/usage",
          },
        ],
      },
      {
        text: "API",
        link: "/api/",
        items: [
          {
            text: "Dvchevskii.Unit",
            link: "/api/Dvchevskii.Unit/",
            items: [
              {
                text: "Unit",
                link: "/api/Dvchevskii.Unit/Unit",
              },
            ],
          },
        ],
      },
    ],

    socialLinks: [
      { icon: "github", link: "https://github.com/2chevskii/unit" },
      {
        icon: {
          svg: nugetIcon,
        },
        link: "https://www.nuget.org/packages/Dvchevskii.Unit",
      },
    ],

    search: { provider: "local" },
  },
});
