# Doctor Profile Creation Guide

## 🔄 **Understanding the Doctor Registration Flow**

### **Current Situation:**
You have users with role "Doctor" but no doctor profiles created yet.

**User Record (✅ Exists):**
```json
{
  "Id": "68ba726f9c3db255782a85d2",
  "Name": "string", 
  "Email": "d@gmail.com",
  "Role": "Doctor",
  "Phone": "string",
  "IsActive": true,
  "CreatedAt": "2025-09-05T05:17:35.861Z"
}
```

**Doctor Profile (❌ Missing):**
```json
// This doesn't exist yet - needs to be created
{
  "id": "doctorProfileId",
  "userId": "68ba726f9c3db255782a85d2",
  "specialtyId": "someSpecialtyId",
  "licenseNumber": "MD123456",
  "experience": 5,
  "qualification": "MBBS",
  "consultationFee": 150.00,
  "isApproved": false,
  "isActive": true
}
```

---

## 🚀 **Step-by-Step Solution:**

### **Step 1: Create a Medical Specialty First**
```bash
POST http://localhost:5289/api/specialties
Content-Type: application/json

{
  "name": "General Medicine",
  "description": "General medical practice and consultation"
}
```

**Response (Save the specialty ID):**
```json
{
  "id": "66d9f1a2b8c7d4e5f6789xyz",
  "name": "General Medicine", 
  "description": "General medical practice and consultation",
  "isActive": true,
  "createdAt": "2025-09-05T10:30:00Z"
}
```

### **Step 2: Create Doctor Profile**
```bash
POST http://localhost:5289/api/doctors
Content-Type: application/json

{
  "userId": "68ba726f9c3db255782a85d2",
  "specialtyId": "66d9f1a2b8c7d4e5f6789xyz",
  "licenseNumber": "MD123456",
  "experience": 3,
  "qualification": "MBBS",
  "consultationFee": 100.00,
  "availability": [
    {
      "dayOfWeek": 1,
      "startTime": "09:00:00",
      "endTime": "17:00:00",
      "isAvailable": true
    },
    {
      "dayOfWeek": 2, 
      "startTime": "09:00:00",
      "endTime": "17:00:00",
      "isAvailable": true
    }
  ]
}
```

**Response:**
```json
{
  "id": "newDoctorProfileId",
  "userId": "68ba726f9c3db255782a85d2",
  "specialtyId": "66d9f1a2b8c7d4e5f6789xyz", 
  "licenseNumber": "MD123456",
  "experience": 3,
  "qualification": "MBBS",
  "consultationFee": 100.00,
  "isApproved": false,
  "isActive": true,
  "createdAt": "2025-09-05T10:35:00Z"
}
```

### **Step 3: Admin Approves Doctor**
```bash
PATCH http://localhost:5289/api/doctors/{newDoctorProfileId}/approve
```

### **Step 4: Now Test the Original API**
```bash
GET http://localhost:5289/api/doctors/user/68ba726f9c3db255782a85d2
```

**Now it should return the doctor profile!** ✅

---

## 🧪 **Complete Test Workflow:**

### **1. Check Current Specialties:**
```bash
GET http://localhost:5289/api/specialties
```

### **2. Create Specialty (if none exist):**
```bash
POST http://localhost:5289/api/specialties
{
  "name": "Cardiology",
  "description": "Heart and cardiovascular disorders"
}
```

### **3. Create Doctor Profile for User 1:**
```bash
POST http://localhost:5289/api/doctors
{
  "userId": "68b9bec7cd231e233de1b8fc",
  "specialtyId": "{specialtyId}",
  "licenseNumber": "MD001",
  "experience": 5,
  "qualification": "MBBS, MD",
  "consultationFee": 150.00,
  "availability": [
    {
      "dayOfWeek": 1,
      "startTime": "09:00:00", 
      "endTime": "17:00:00",
      "isAvailable": true
    }
  ]
}
```

### **4. Create Doctor Profile for User 2:**
```bash
POST http://localhost:5289/api/doctors
{
  "userId": "68ba726f9c3db255782a85d2",
  "specialtyId": "{specialtyId}",
  "licenseNumber": "MD002", 
  "experience": 3,
  "qualification": "MBBS",
  "consultationFee": 100.00,
  "availability": [
    {
      "dayOfWeek": 2,
      "startTime": "10:00:00",
      "endTime": "18:00:00", 
      "isAvailable": true
    }
  ]
}
```

### **5. Test Both APIs:**
```bash
GET http://localhost:5289/api/doctors/user/68b9bec7cd231e233de1b8fc
GET http://localhost:5289/api/doctors/user/68ba726f9c3db255782a85d2
```

---

## 🎯 **Frontend Integration:**

### **Handle Missing Doctor Profile:**
```typescript
const checkDoctorProfile = async (userId: string) => {
  try {
    const doctor = await apiService.getDoctorByUserId(userId);
    console.log('Doctor profile found:', doctor);
    return doctor;
  } catch (error) {
    if (error.response?.status === 404) {
      console.log('Doctor profile not created yet');
      // Redirect to create doctor profile page
      router.push('/create-doctor-profile');
    } else {
      console.error('Error fetching doctor:', error);
    }
  }
};
```

### **Create Doctor Profile Flow:**
```typescript
const createDoctorProfile = async (formData: any) => {
  try {
    const doctorData = {
      userId: currentUser.id,
      specialtyId: formData.specialtyId,
      licenseNumber: formData.licenseNumber,
      experience: formData.experience,
      qualification: formData.qualification,
      consultationFee: formData.consultationFee,
      availability: formData.availability
    };
    
    const doctor = await apiService.createDoctor(doctorData);
    console.log('Doctor profile created:', doctor);
    
    // Show success message
    alert('Doctor profile created! Waiting for admin approval.');
    
  } catch (error) {
    console.error('Error creating doctor profile:', error);
  }
};
```

---

## ✅ **Summary:**

**The API is working correctly!** 

The issue is that you have:
- ✅ **User accounts** with role "Doctor" 
- ❌ **No doctor profiles** created yet

**To fix:**
1. Create medical specialties
2. Create doctor profiles for your doctor users
3. Admin approves the doctor profiles  
4. Then `/api/doctors/user/{userId}` will work

**This is the expected workflow for a doctor appointment system!** 🏥
