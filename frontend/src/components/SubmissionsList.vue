<template>
  <div class="submissions-list">
    <div class="search-container">
      <input
        type="text"
        class="search-input"
        placeholder="Search submissions..."
        v-model="searchQuery"
        @input="debouncedSearch"
      />
    </div>

    <div v-if="loading" class="loading">Loading...</div>

    <div v-else-if="submissions.length === 0" class="loading">
      No submissions found
    </div>

    <table v-else>
      <thead>
        <tr>
          <th>ID</th>
          <th>Form Key</th>
          <th>Created At</th>
          <th>Data Preview</th>
          <th>Actions</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="submission in submissions" :key="submission.id">
          <td>{{ submission.id.substring(0, 8) }}...</td>
          <td>{{ submission.formKey }}</td>
          <td>{{ formatDate(submission.createdAt) }}</td>
          <td class="data-preview">{{ getDataPreview(submission.data) }}</td>
          <td>
            <button class="btn-view-json" @click="viewJson(submission)">View JSON</button>
          </td>
        </tr>
      </tbody>
    </table>

    <div v-if="selectedSubmission" class="json-modal" @click.self="closeJsonModal">
      <div class="json-modal-content">
        <div class="json-modal-header">
          <h3>Submission JSON</h3>
          <button class="btn-close" @click="closeJsonModal">×</button>
        </div>
        <pre class="json-content">{{ formatJson(selectedSubmission.data) }}</pre>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, defineExpose, onBeforeUnmount } from 'vue'

const submissions = ref([])
const searchQuery = ref('')
const loading = ref(false)
const selectedSubmission = ref(null)
let searchTimeout = null

const debouncedSearch = () => {
  if (searchTimeout) {
    clearTimeout(searchTimeout)
  }
  searchTimeout = setTimeout(() => {
    fetchSubmissions()
  }, 300)
}

const fetchSubmissions = async () => {
  loading.value = true
  try {
    let url = '/api/submissions'
    if (searchQuery.value) {
      url += `?search=${encodeURIComponent(searchQuery.value.trim())}`
    }
    
    const response = await fetch(url)
    if (!response.ok) {
      throw new Error(`Failed to fetch submissions: ${response.status}`)
    }
    
    const data = await response.json()
    submissions.value = data || []
    
  } catch (error) {
    console.error('Error fetching submissions:', error)
    submissions.value = []
  } finally {
    loading.value = false
  }
}

const formatDate = (dateString) => {
  if (!dateString) return 'N/A'
  
  try {
    const date = new Date(dateString)
    if (isNaN(date.getTime())) {
      return dateString
    }
    return date.toLocaleString()
  } catch {
    return dateString
  }
}

const getDataPreview = (dataJson) => {
  if (!dataJson) return 'No data'
  
  try {
    const data = JSON.parse(dataJson)
    const parts = []
    const priorityFields = ['name', 'email', 'title', 'subject', 'message', 'topic', 'description']
    
    for (const field of priorityFields) {
      if (data[field] && typeof data[field] === 'string') {
        const value = data[field]
        const truncated = value.length > 30 ? `${value.substring(0, 30)}...` : value
        parts.push(`${field}: ${truncated}`)
        if (parts.length >= 2) break
      }
    }
    
    if (parts.length === 0) {
      const keys = Object.keys(data)
      for (let i = 0; i < Math.min(2, keys.length); i++) {
        const key = keys[i]
        const value = data[key]
        if (value !== null && value !== undefined) {
          const str = typeof value === 'object' 
            ? JSON.stringify(value) 
            : String(value)
          const truncated = str.length > 30 ? `${str.substring(0, 30)}...` : str
          parts.push(`${key}: ${truncated}`)
        }
      }
    }
    
    return parts.length > 0 ? parts.join(', ') : 'No preview available'
  } catch {
    const maxLength = 50
    return dataJson.length > maxLength 
      ? `${dataJson.substring(0, maxLength)}...` 
      : dataJson
  }
}

const viewJson = (submission) => {
  selectedSubmission.value = submission
}

const closeJsonModal = () => {
  selectedSubmission.value = null
}

const formatJson = (jsonString) => {
  if (!jsonString) return ''
  
  try {
    const obj = JSON.parse(jsonString)
    return JSON.stringify(obj, null, 2)
  } catch {
    return jsonString
  }
}

onBeforeUnmount(() => {
  if (searchTimeout) {
    clearTimeout(searchTimeout)
  }
})

defineExpose({
  refresh: fetchSubmissions
})

onMounted(() => {
  fetchSubmissions()
})
</script>

<style scoped>
.btn-view-json {
  background-color: #6c757d;
  color: white;
  padding: 6px 12px;
  border: none;
  border-radius: 4px;
  font-size: 12px;
  cursor: pointer;
  transition: background-color 0.2s;
}

.btn-view-json:hover {
  background-color: #5a6268;
}

.json-modal {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.json-modal-content {
  background: white;
  border-radius: 8px;
  padding: 20px;
  max-width: 80%;
  max-height: 80%;
  overflow: auto;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.json-modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 15px;
  border-bottom: 1px solid #ddd;
  padding-bottom: 10px;
}

.json-modal-header h3 {
  margin: 0;
}

.btn-close {
  background: none;
  border: none;
  font-size: 24px;
  cursor: pointer;
  color: #666;
  padding: 0;
  width: 30px;
  height: 30px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.btn-close:hover {
  color: #000;
}

.json-content {
  background-color: #f8f9fa;
  padding: 15px;
  border-radius: 4px;
  overflow-x: auto;
  font-family: 'Courier New', monospace;
  font-size: 13px;
  line-height: 1.5;
  margin: 0;
}
</style>

