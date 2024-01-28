import { defineConfig } from "vitepress";
import nugetIcon from "./resources/nuget-icon.js";
import pkg from "../package.json";

export default defineConfig({
  base: "/unit/",
  lang: "en-US",
  head: [["link", { rel: "icon", href: `/unit/favicon.ico` }]],
  title: "Dvchevskii.Unit",
  description: "Dvchevskii.Unit package documentation",
  appearance: "force-dark",
  themeConfig: {
    search: { provider: "local" },
    nav: [
      { text: "Home", link: "/" },
      { text: "Guide", link: "/guide/installation" },
      { text: `API`, link: "/api/Dvchevskii.Unit/" },
      {
        text: pkg["latestReleaseVersion"],
        items: [
          {
            text: "Release notes",
            link:
              "https://github.com/2chevskii/unit/releases/tag/v" +
              pkg["latestReleaseVersion"],
          },
        ],
      },
    ],
    socialLinks: [
      { icon: "github", link: "https://github.com/2chevskii/unit" },
      {
        icon: { svg: nugetIcon },
        link: "https://www.nuget.org/packages/Dvchevskii.Unit",
      },
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
    footer: {
      message: `Built with <a href="https://vitepress.dev">VitePress</a> | API version: <code>${pkg["version"]}</code><br/>Released under the MIT license. All rights reserved`,
    },
  },
});
