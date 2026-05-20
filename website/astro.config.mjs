// @ts-check
import { defineConfig } from 'astro/config';
import tailwindcss from '@tailwindcss/vite';
import mdx from '@astrojs/mdx';

import sitemap from '@astrojs/sitemap';

// Redirect map for per-chart docs: pages mostly live at /docs/charts/<name>-chart
// but users commonly type the shorter form. Each entry redirects the shorter URL
// to the actual page so 404s like "/docs/charts/line" don't bounce visitors.
// (candlestick, heatmap, and box-plot already use the short slug.)
const chartRedirects = [
  'line', 'bar', 'pie', 'donut', 'scatter', 'bubble', 'area', 'range-area',
  'radar', 'gauge', 'funnel', 'treemap', 'waterfall',
  'rose', 'sankey', 'chord', 'stacked-bar',
].reduce((acc, name) => {
  acc[`/docs/charts/${name}`] = `/docs/charts/${name}-chart`;
  return acc;
}, /** @type {Record<string,string>} */ ({}));

export default defineConfig({
  site: 'https://arcadiaui.com',
  integrations: [mdx(), sitemap()],
  output: 'static',
  redirects: {
    ...chartRedirects,
    '/docs/charts/realtime': '/docs/charts/streaming',
  },
  vite: {
    plugins: [tailwindcss()],
  },
});