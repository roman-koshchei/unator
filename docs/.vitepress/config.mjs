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
        // items: [
        //   { text: "Markdown Examples", link: "/markdown-examples" },
        //   { text: "Runtime API Examples", link: "/api-examples" },
        // ],
      },
      {
        text: "Getting Started",
        link: "/getting-started",
        // items: [
        //   { text: "Markdown Examples", link: "/markdown-examples" },
        //   { text: "Runtime API Examples", link: "/api-examples" },
        // ],
      },
    ],

    socialLinks: [
      { icon: "github", link: "https://github.com/roman-koshchei/unator" },
    ],
  },
})
