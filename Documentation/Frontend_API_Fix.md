# Frontend API Integration Fix

## 🔧 **Problem Solved: UserID "undefined" Error Fixed**

### **Original Error:**
```
AxiosError: Request failed with status code 500
Error retrieving doctor for user ID: undefined
System.FormatException: 'undefined' is not a valid 24 digit hex string.
```

### **Root Cause:**
The frontend was sending `"undefined"` as the userId parameter to the backend, which MongoDB couldn't parse as a valid ObjectId.

---

## ✅ **Backend Fix Applied:**

### **1. Added Input Validation**
```csharp
// Added to DoctorsController.cs
private bool IsValidObjectId(string id)
{
    return !string.IsNullOrWhiteSpace(id) && 
           id != "undefined" && 
           id != "null" && 
           MongoDB.Bson.ObjectId.TryParse(id, out _);
}
```

### **2. Updated Controller Methods**
```csharp
[HttpGet("user/{userId}")]
public async Task<ActionResult<DoctorDto>> GetDoctorByUserId(string userId)
{
    try
    {
        if (!IsValidObjectId(userId))
        {
            return BadRequest(new { message = "Invalid user ID format. Must be a valid MongoDB ObjectId" });
        }

        var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);
        if (doctor == null)
            return NotFound(new { message = "Doctor profile not found for this user" });

        return Ok(doctor);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error retrieving doctor for user ID: {UserId}", userId);
        return StatusCode(500, new { message = "An error occurred while retrieving the doctor profile" });
    }
}
```

---

## 🚀 **Frontend Fix Needed:**

### **Issue in your Frontend:**
The problem is in your `ApiService.getDoctorByUserId()` method. You're passing `undefined` as the userId.

### **Check these locations in your frontend:**

#### **1. Verify User ID is Available**
```typescript
// Before calling getDoctorByUserId, ensure userId is valid:
if (!userId || userId === 'undefined' || userId === 'null') {
    console.error('Invalid userId:', userId);
    return;
}

const doctor = await apiService.getDoctorByUserId(userId);
```

#### **2. Check Authentication State**
```typescript
// Make sure user is logged in and has an ID
const user = getCurrentUser(); // Your method to get current user
if (!user || !user.id) {
    console.error('User not logged in or missing ID');
    return;
}

const doctor = await apiService.getDoctorByUserId(user.id);
```

#### **3. Update Your API Service**
```typescript
// src/lib/api.ts
async getDoctorByUserId(userId: string): Promise<Doctor> {
    // Add validation
    if (!userId || userId === 'undefined' || userId === 'null') {
        throw new Error('Valid user ID is required');
    }
    
    // Validate MongoDB ObjectId format (24 hex characters)
    const objectIdRegex = /^[0-9a-fA-F]{24}$/;
    if (!objectIdRegex.test(userId)) {
        throw new Error('Invalid user ID format');
    }
    
    const response = await this.api.get(`/doctors/user/${userId}`);
    return response.data;
}
```

---

## 🧪 **Testing the Fix:**

### **1. Test with Invalid IDs (Should return 400 Bad Request):**
```bash
GET http://localhost:5289/api/doctors/user/undefined
GET http://localhost:5289/api/doctors/user/null  
GET http://localhost:5289/api/doctors/user/invalid-id
```

**Expected Response:**
```json
{
  "message": "Invalid user ID format. Must be a valid MongoDB ObjectId"
}
```

### **2. Test with Valid ID (Should work):**
```bash
GET http://localhost:5289/api/doctors/user/66d9f1a2b8c7d4e5f6789abc
```

**Expected Response:**
```json
{
  "id": "doctorId123",
  "userId": "66d9f1a2b8c7d4e5f6789abc",
  "specialtyId": "specialtyId123",
  "licenseNumber": "MD123456",
  "experience": 5,
  "qualification": "MBBS",
  "consultationFee": 150.00,
  // ... other doctor data
}
```

### **3. Test Non-Existent User (Should return 404):**
```bash
GET http://localhost:5289/api/doctors/user/507f1f77bcf86cd799439011
```

**Expected Response:**
```json
{
  "message": "Doctor profile not found for this user"
}
```

---

## 🔍 **Debug Your Frontend:**

### **1. Add Console Logging:**
```typescript
// Add this before calling the API:
console.log('About to call getDoctorByUserId with userId:', userId);
console.log('typeof userId:', typeof userId);
console.log('userId length:', userId?.length);

// Check if userId is coming from the right source
const user = getCurrentUser();
console.log('Current user object:', user);
console.log('User ID from user object:', user?.id);
```

### **2. Check Authentication Flow:**
```typescript
// Make sure login is working properly
const loginResponse = await apiService.login(email, password);
console.log('Login response:', loginResponse);

// Ensure user ID is being stored correctly
if (loginResponse.user && loginResponse.user.id) {
    // Store user ID properly
    localStorage.setItem('userId', loginResponse.user.id);
    // or however you're managing state
}
```

### **3. Verify User Context:**
```typescript
// In your component where you call getDoctorByUserId:
useEffect(() => {
    const checkUser = async () => {
        const currentUser = getCurrentUser();
        console.log('Current user in component:', currentUser);
        
        if (currentUser && currentUser.id) {
            try {
                const doctor = await apiService.getDoctorByUserId(currentUser.id);
                console.log('Doctor found:', doctor);
            } catch (error) {
                console.error('Error fetching doctor:', error);
            }
        } else {
            console.log('No valid user found');
        }
    };
    
    checkUser();
}, []);
```

---

## 📋 **Complete Workflow Example:**

### **1. User Registration/Login:**
```typescript
// Register or login
const loginResult = await apiService.login(email, password);
// loginResult.user.id should be a valid MongoDB ObjectId like "66d9f1a2b8c7d4e5f6789abc"
```

### **2. Check if User is Doctor:**
```typescript
if (loginResult.user.role === 'Doctor') {
    // Try to get doctor profile
    try {
        const doctor = await apiService.getDoctorByUserId(loginResult.user.id);
        console.log('Doctor profile found:', doctor);
    } catch (error) {
        if (error.response?.status === 404) {
            console.log('Doctor profile not created yet');
            // Redirect to create doctor profile
        } else {
            console.error('Error fetching doctor profile:', error);
        }
    }
}
```

### **3. Create Doctor Profile if Doesn't Exist:**
```typescript
const createDoctorProfile = async (userId: string) => {
    const doctorData = {
        userId: userId,
        specialtyId: "selectedSpecialtyId",
        licenseNumber: "MD123456",
        experience: 5,
        qualification: "MBBS",
        consultationFee: 150.00,
        availability: [
            {
                dayOfWeek: 1, // Monday
                startTime: "09:00:00",
                endTime: "17:00:00",
                isAvailable: true
            }
        ]
    };
    
    try {
        const doctor = await apiService.createDoctor(doctorData);
        console.log('Doctor profile created:', doctor);
        return doctor;
    } catch (error) {
        console.error('Error creating doctor profile:', error);
        throw error;
    }
};
```

---

## ✅ **Backend is Now Fixed:**

- ✅ **Input validation** added for all ID parameters
- ✅ **Proper error messages** for invalid IDs  
- ✅ **MongoDB ObjectId validation** prevents crashes
- ✅ **Handles "undefined", "null", and invalid formats**
- ✅ **Returns meaningful HTTP status codes**

**Your frontend should now receive proper error responses instead of 500 crashes!**

---

## 🎯 **Next Steps:**

1. **Check your frontend userId source** - ensure it's not undefined
2. **Add validation in frontend** before calling API
3. **Handle different response codes** (400, 404, 500)
4. **Implement proper error handling** in your frontend
5. **Test the complete flow** from login to doctor profile fetching

**The backend is now robust and ready for frontend integration!** 🚀
