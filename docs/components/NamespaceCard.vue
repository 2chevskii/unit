<script setup>
defineProps(["namespace"]);
</script>

<template>
  <div class="namespace-card">
    <h2>
      <a :href="namespace.link">{{ namespace.name }}</a>
    </h2>
    <ul>
      <li v-for="typeDef in namespace.types" :key="typeDef.name">
        <a :href="typeDef.link">{{ typeDef.name }}</a>
      </li>
    </ul>
  </div>
</template>

<style scoped>
.namespace-card {
  width: fit-content;
  background-color: var(--vp-c-bg-soft);
  padding: 24px;
  border-radius: 12px;
}

h2 {
  border: none;
  margin: 0;
  padding: 0;
}

a {
  font-weight: bold;
  text-decoration: none;
}

li > ul > li {
  list-style: circle;
}
</style>
