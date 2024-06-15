import { defineConfig } from "vitepress"

// https://vitepress.dev/reference/site-config
export default defineConfig({
  title: "Unator",
  description:
    "C# utilities, such as typesafe router. Breaking harmful standards is fie.",

  base: "/unator/",
  sitemap: {
    hostname: "https://roman-koshchei.github.io",
  },
  cleanUrls: true,

  themeConfig: {
    // https://vitepress.dev/reference/default-theme-config
    nav: [{ text: "Introduction", link: "/introduction" }],

    sidebar: [
      {
        text: "Introduction",
        link: "/introduction",
      },
      {
        text: "Getting Started",
        link: "/getting-started",
      },
      {
        text: "Env",
        link: "/env",
      },
      {
        text: "Switches",
        items: [
          { text: "Email", link: "/switches/email" },
          { text: "Storage", link: "/switches/storage" },
        ],
      },
      {
        text: "MVC Extensions",
        items: [
          { text: "Partial", link: "/mvc/partial" },
        ],
      },
      {
        text: "Experimental",
        items: [
          { text: "Database", link: "/experimental/database" },
          { text: "Router", link: "/experimental/router" },
        ],
      },
      {
        text: "Showcase",
        items: [{ text: "Spentoday", link: "/showcase/spentoday" }],
      },
    ],

    socialLinks: [
      { icon: "github", link: "https://github.com/roman-koshchei/unator" },
      { icon: "twitter", link: "https://twitter.com/roman_koshchei" },
    ],
  },
})
