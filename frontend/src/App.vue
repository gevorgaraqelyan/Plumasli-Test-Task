<template>
  <div id="app">
    <h1>Form Submissions</h1>
    
    <div class="form-container">
      <h2>Contact / Order Form</h2>
      <FormSubmit @submitted="handleSubmission" />
    </div>

    <div class="submissions-container">
      <h2>Submissions</h2>
      <SubmissionsList ref="submissionsListRef" />
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import FormSubmit from './components/FormSubmit.vue'
import SubmissionsList from './components/SubmissionsList.vue'

const submissionsListRef = ref(null)

const handleSubmission = () => {
  if (submissionsListRef.value) {
    submissionsListRef.value.refresh()
  }
}
</script>

