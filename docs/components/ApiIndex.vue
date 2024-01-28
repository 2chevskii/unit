<script setup>
import NamespaceCard from "./NamespaceCard.vue";
import { useData } from "vitepress";

const { theme } = useData();

const typeData = theme.value.sidebar
  .filter((x) => x.link === "/api/")[0]
  .items.map((ns) => ({
    name: ns.text,
    link: ns.link,
    types: ns.items.map((type) => ({
      name: type.text,
      link: type.link,
    })),
  }));
</script>

<template>
  <h1 class="header">API Reference</h1>
  <div class="namespace-list">
    <NamespaceCard v-for="ns in typeData" :key="ns.name" :namespace="ns">
    </NamespaceCard>
  </div>
</template>

<style scoped>
.header {
  color: var(--vp-home-hero-name-color);
}

.namespace-list {
  margin-top: 4rem;
  display: flex;
  flex-wrap: wrap;
  justify-content: space-between;
}

.namespace-list > * {
  flex-basis: 45%;
}
</style>
