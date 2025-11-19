<template>
  <form @submit.prevent="handleSubmit">
    <div class="form-group">
      <label for="name">Name <span class="required">*</span></label>
      <input
        type="text"
        id="name"
        v-model="formData.name"
        @blur="validateField('name')"
      />
      <span v-if="errors.name" class="error-message">{{ errors.name }}</span>
    </div>

    <div class="form-group">
      <label for="email">Email <span class="required">*</span></label>
      <input
        type="email"
        id="email"
        v-model="formData.email"
        @blur="validateField('email')"
      />
      <span v-if="errors.email" class="error-message">{{ errors.email }}</span>
    </div>

    <div class="form-group">
      <label for="topic">Topic <span class="required">*</span></label>
      <select
        id="topic"
        v-model="formData.topic"
        @blur="validateField('topic')"
      >
        <option value="">Select a topic</option>
        <option value="general">General Inquiry</option>
        <option value="support">Support</option>
        <option value="sales">Sales</option>
        <option value="feedback">Feedback</option>
      </select>
      <span v-if="errors.topic" class="error-message">{{ errors.topic }}</span>
    </div>

    <div class="form-group">
      <label for="preferredDate">Preferred Date <span class="required">*</span></label>
      <input
        type="date"
        id="preferredDate"
        v-model="formData.preferredDate"
        @blur="validateField('preferredDate')"
      />
      <span v-if="errors.preferredDate" class="error-message">{{ errors.preferredDate }}</span>
    </div>

    <div class="form-group">
      <label>Preferred Contact Method <span class="required">*</span></label>
      <div class="radio-group">
        <div class="radio-option">
          <input
            type="radio"
            id="email-contact"
            value="email"
            v-model="formData.contactMethod"
            @blur="validateField('contactMethod')"
          />
          <label for="email-contact">Email</label>
        </div>
        <div class="radio-option">
          <input
            type="radio"
            id="phone-contact"
            value="phone"
            v-model="formData.contactMethod"
            @blur="validateField('contactMethod')"
          />
          <label for="phone-contact">Phone</label>
        </div>
        <div class="radio-option">
          <input
            type="radio"
            id="sms-contact"
            value="sms"
            v-model="formData.contactMethod"
            @blur="validateField('contactMethod')"
          />
          <label for="sms-contact">SMS</label>
        </div>
      </div>
      <span v-if="errors.contactMethod" class="error-message">{{ errors.contactMethod }}</span>
    </div>

    <div class="form-group">
      <div class="checkbox-group">
        <div class="checkbox-option">
          <input
            type="checkbox"
            id="acceptTerms"
            v-model="formData.acceptTerms"
            @blur="validateField('acceptTerms')"
          />
          <label for="acceptTerms">I accept the terms and conditions <span class="required">*</span></label>
        </div>
      </div>
      <span v-if="errors.acceptTerms" class="error-message">{{ errors.acceptTerms }}</span>
    </div>

    <div v-if="successMessage" class="success-message">
      {{ successMessage }}
    </div>

    <div v-if="errorMessage" class="error-alert">
      {{ errorMessage }}
    </div>

    <button type="submit" :disabled="isSubmitting">
      {{ isSubmitting ? 'Submitting...' : 'Submit' }}
    </button>
  </form>
</template>

<script setup>
import { ref, reactive } from 'vue'

const emit = defineEmits(['submitted'])

const formData = reactive({
  name: '',
  email: '',
  topic: '',
  preferredDate: '',
  contactMethod: '',
  acceptTerms: false
})

const errors = reactive({})
const successMessage = ref('')
const errorMessage = ref('')
const isSubmitting = ref(false)

const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/

const validateField = (fieldName) => {
  delete errors[fieldName]

  switch (fieldName) {
    case 'name':
      if (!formData.name.trim()) {
        errors.name = 'Name is required'
      }
      break

    case 'email':
      const email = formData.email.trim()
      if (!email) {
        errors.email = 'Email is required'
      } else if (!emailRegex.test(email)) {
        errors.email = 'Please enter a valid email address'
      }
      break

    case 'topic':
      if (!formData.topic) {
        errors.topic = 'Please select a topic'
      }
      break

    case 'preferredDate':
      if (!formData.preferredDate) {
        errors.preferredDate = 'Preferred date is required'
      } else {
        const selectedDate = new Date(formData.preferredDate)
        const today = new Date()
        today.setHours(0, 0, 0, 0)
        if (selectedDate < today) {
          errors.preferredDate = 'Date cannot be in the past'
        }
      }
      break

    case 'contactMethod':
      if (!formData.contactMethod) {
        errors.contactMethod = 'Please select a contact method'
      }
      break

    case 'acceptTerms':
      if (!formData.acceptTerms) {
        errors.acceptTerms = 'You must accept the terms and conditions'
      }
      break
  }
}

const validateForm = () => {
  validateField('name')
  validateField('email')
  validateField('topic')
  validateField('preferredDate')
  validateField('contactMethod')
  validateField('acceptTerms')

  return Object.keys(errors).length === 0
}

const handleSubmit = async () => {
  successMessage.value = ''
  errorMessage.value = ''

  if (!validateForm()) {
    return
  }

  isSubmitting.value = true

  try {
    const response = await fetch('/api/forms/contact/submissions', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        name: formData.name.trim(),
        email: formData.email.trim(),
        topic: formData.topic,
        preferredDate: formData.preferredDate,
        contactMethod: formData.contactMethod,
        acceptTerms: formData.acceptTerms
      })
    })

    if (!response.ok) {
      let errorText = 'Failed to submit form'
      try {
        const error = await response.json()
        errorText = error.error || errorText
      } catch {
        console.error('Error parsing error response')
      }
      throw new Error(errorText)
    }

    successMessage.value = 'Form submitted successfully!'
    
    formData.name = ''
    formData.email = ''
    formData.topic = ''
    formData.preferredDate = ''
    formData.contactMethod = ''
    formData.acceptTerms = false
    Object.keys(errors).forEach(key => delete errors[key])

    emit('submitted')
    
    setTimeout(() => {
      successMessage.value = ''
    }, 5000)

  } catch (error) {
    errorMessage.value = error.message || 'An error occurred while submitting the form'
    console.error('Submit error:', error)
  } finally {
    isSubmitting.value = false
  }
}
</script>

<style scoped>
.required {
  color: #e74c3c;
}
</style>

