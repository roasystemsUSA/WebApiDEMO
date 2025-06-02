using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoaSystems.Libraries.Model;
using RoaSystems.Libraries.Model.Base;
using RoaSystems.Libraries.Services;

namespace RoaSystems.WebAPIDemo.API.Controllers
{
    public enum eOperation
    {
        CREATE,
        SELECT,
        UPDATE,
        DELETE,
        UNDELETE
    }

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ApplicationController : ControllerBase
    {
        private readonly IApplicationService _applicationsService;

        public ApplicationController(IApplicationService applicationsService)
        {
            _applicationsService = applicationsService;
        }


        // GET: api/Applications
        [HttpGet]
        //[AllowAnonymous] // Allow anonymous access for this endpoint
        public async Task<BaseResponseModel<IEnumerable<Application>>> GetApplications()
        {
            try
            {
                var languages = await _applicationsService.GetAllAsync();
                if (languages == null || !languages.Any())
                {
                    return new BaseResponseModel<IEnumerable<Application>>()
                    {
                        ErrorDetails = new ErrorDetailsModel()
                        {
                            ErrorType = ErrorDetailsModel.ErrorTypeEnum.Internal,
                            HttpStatusCode = HttpStatusCodeEnum.NotFound,
                            Message = "No applications found"
                        },
                        HasError = false,
                        Result = null
                    };
                }
                return new BaseResponseModel<IEnumerable<Application>>()
                {
                    Result = languages,
                    ErrorDetails = new ErrorDetailsModel() { HttpStatusCode = HttpStatusCodeEnum.OK, Message = "Success" },
                    HasError = false
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseModel<IEnumerable<Application>>()
                {
                    ErrorDetails = new ErrorDetailsModel()
                    {
                        ErrorType = ErrorDetailsModel.ErrorTypeEnum.Internal,
                        HttpStatusCode = HttpStatusCodeEnum.Internal,
                        Message = ex.Message
                    },
                    HasError = true,
                    Result = null
                };
            }
        }

        // GET: api/Applications/5
        [HttpGet("{id}")]
        public async Task<BaseResponseModel<Application>> GetApplication(int id)
        {
            try
            {
                var result = await _applicationsService.GetByIdAsync(id);
                if (result == null)
                {
                    return new BaseResponseModel<Application>()
                    {
                        ErrorDetails = new ErrorDetailsModel()
                        {
                            ErrorType = ErrorDetailsModel.ErrorTypeEnum.Internal,
                            HttpStatusCode = HttpStatusCodeEnum.NotFound,
                            Message = "Application not found"
                        },
                        HasError = true,
                        Result = null
                    };
                }
                // Create a response model
                BaseResponseModel<Application> response = new BaseResponseModel<Application>()
                {
                    Result = result,
                    ErrorDetails = new ErrorDetailsModel() { HttpStatusCode = HttpStatusCodeEnum.OK, Message = "Success" },
                    HasError = false
                };
                return response;
            }
            catch (Exception ex)
            {
                return new BaseResponseModel<Application>()
                {
                    ErrorDetails = new ErrorDetailsModel()
                    {
                        ErrorType = ErrorDetailsModel.ErrorTypeEnum.Internal,
                        HttpStatusCode = HttpStatusCodeEnum.Internal,
                        Message = ex.Message
                    },
                    HasError = true,
                    Result = null
                };
            }
        }

        // POST: api/Applications
        [HttpPost]
        public async Task<BaseResponseModel<Application>> PostApplication(Application application)
        {
            try
            {

                // Add the application to the database
                var insertedApplication = await _applicationsService.CreateAsync(application);


                BaseResponseModel<Application> result = new BaseResponseModel<Application>()
                {
                    Result = insertedApplication,
                    ErrorDetails = new ErrorDetailsModel() { HttpStatusCode = HttpStatusCodeEnum.OK, Message = "Success" },
                    HasError = false
                };

                return result;
            }
            catch (Exception ex)
            {
                return new BaseResponseModel<Application>()
                {
                    ErrorDetails = new ErrorDetailsModel()
                    {
                        ErrorType = ErrorDetailsModel.ErrorTypeEnum.Internal,
                        HttpStatusCode = HttpStatusCodeEnum.Internal,
                        Message = $"An error occurred while creating the application. Details: {ex.Message}"
                    },
                    HasError = true,
                    Result = null
                };
            }
        }

        // PUT: api/Applications/5
        [HttpPut("{id}")]
        public async Task<BaseResponseModel<bool?>> PutApplication(int id, Application application)
        {
            try
            {
                // update the existing application
                var updateResult = await _applicationsService.UpdateAsync(id, application);
                BaseResponseModel<bool?> result = new BaseResponseModel<bool?>()
                {
                    Result = updateResult,
                    ErrorDetails = new ErrorDetailsModel() { HttpStatusCode = HttpStatusCodeEnum.OK, Message = "Success" },
                    HasError = !updateResult
                };
                return result;
            }
            catch (Exception ex)
            {
                return new BaseResponseModel<bool?>()
                {
                    ErrorDetails = new ErrorDetailsModel()
                    {
                        ErrorType = ErrorDetailsModel.ErrorTypeEnum.Internal,
                        HttpStatusCode = HttpStatusCodeEnum.Internal,
                        Message = $"An error occurred while updating the application. Details: {ex.Message}"
                    },
                    HasError = true,
                    Result = null
                };
            }


        }

        /// <summary>
        /// Recovers an Application that previously was logically deleted
        /// </summary>
        /// <param name="id">Application ID</param>
        /// <returns>The success status of the undelete operation</returns>
        [HttpPut("undelete/{id}")]
        public async Task<BaseResponseModel<bool?>> Undelete(int id)
        {
            try
            {
                var application = await _applicationsService.GetByIdAsync(id);
                if (application == null)
                {
                    return new BaseResponseModel<bool?>()
                    {
                        ErrorDetails = new ErrorDetailsModel()
                        {
                            ErrorType = ErrorDetailsModel.ErrorTypeEnum.Internal,
                            HttpStatusCode = HttpStatusCodeEnum.NotFound,
                            Message = $"Application with ID {id} was not found."
                        },
                        HasError = true,
                        Result = null
                    };
                }

                SetAuditValues(application, eOperation.UNDELETE);
                var updateResult = await _applicationsService.UpdateAsync(id, application);

                return new BaseResponseModel<bool?>()
                {
                    Result = updateResult,
                    ErrorDetails = new ErrorDetailsModel()
                    {
                        HttpStatusCode = HttpStatusCodeEnum.OK,
                        Message = updateResult ? "Application successfully recovered." : "Failed to recover application."
                    },
                    HasError = !updateResult
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseModel<bool?>()
                {
                    ErrorDetails = new ErrorDetailsModel()
                    {
                        ErrorType = ErrorDetailsModel.ErrorTypeEnum.Internal,
                        HttpStatusCode = HttpStatusCodeEnum.Internal,
                        Message = $"An error occurred while recovering the application. Details: {ex.Message}"
                    },
                    HasError = true,
                    Result = null
                };
            }
        }

        /// <summary>
        /// Deletes an Application, either physically or logically depending on the flag
        /// </summary>
        /// <param name="id">Application ID</param>
        /// <param name="isPhysicalDelete">If true, deletes the application from the database; otherwise, performs a logical (soft) delete</param>
        /// <returns>Boolean indicating success or failure of the delete operation</returns>
        [HttpDelete("{id}")]
        public async Task<BaseResponseModel<bool?>> DeleteApplication(int id, bool isPhysicalDelete)
        {
            try
            {
                bool deleteResult;

                if (isPhysicalDelete)
                {
                    // Perform physical delete logic
                    deleteResult = await _applicationsService.DeleteAsync(id);
                    if (!deleteResult)
                    {
                        return new BaseResponseModel<bool?>()
                        {
                            ErrorDetails = new ErrorDetailsModel()
                            {
                                ErrorType = ErrorDetailsModel.ErrorTypeEnum.Internal,
                                HttpStatusCode = HttpStatusCodeEnum.NotFound,
                                Message = $"Application with ID {id} was not found for physical deletion."
                            },
                            HasError = true,
                            Result = false
                        };
                    }
                }
                else
                {
                    // Perform soft delete logic
                    var application = await _applicationsService.GetByIdAsync(id);
                    if (application == null)
                    {
                        return new BaseResponseModel<bool?>()
                        {
                            ErrorDetails = new ErrorDetailsModel()
                            {
                                ErrorType = ErrorDetailsModel.ErrorTypeEnum.Internal,
                                HttpStatusCode = HttpStatusCodeEnum.NotFound,
                                Message = $"Application with ID {id} was not found for logical deletion."
                            },
                            HasError = true,
                            Result = false
                        };
                    }

                    SetAuditValues(application, eOperation.DELETE);
                    deleteResult = await _applicationsService.UpdateAsync(id, application);
                }

                return new BaseResponseModel<bool?>()
                {
                    Result = deleteResult,
                    ErrorDetails = new ErrorDetailsModel()
                    {
                        HttpStatusCode = HttpStatusCodeEnum.OK,
                        Message = deleteResult
                            ? $"Application successfully {(isPhysicalDelete ? "physically" : "logically")} deleted."
                            : $"Failed to {(isPhysicalDelete ? "physically" : "logically")} delete the application."
                    },
                    HasError = !deleteResult
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseModel<bool?>()
                {
                    ErrorDetails = new ErrorDetailsModel()
                    {
                        ErrorType = ErrorDetailsModel.ErrorTypeEnum.Internal,
                        HttpStatusCode = HttpStatusCodeEnum.Internal,
                        Message = $"An error occurred while {(isPhysicalDelete ? "physically" : "logically")} deleting the application. Details: {ex.Message}"
                    },
                    HasError = true,
                    Result = null
                };
            }
        }

        private void SetAuditValues(Application application, eOperation operation)
        {
            switch (operation)
            {
                case eOperation.CREATE:
                    application.CreationDate = DateTime.UtcNow;
                    application.Deleted = false;
                    application.DeletedDate = null;
                    application.DeletedBy = null;
                    application.LastUpdateDate = null;
                    application.LastUpdatedBy = null;
                    application.CreatedBy = User.Identity.Name; // Set the created by user
                    break;
                case eOperation.UPDATE:
                    application.LastUpdateDate = DateTime.UtcNow;
                    application.LastUpdatedBy = User.Identity.Name; // Set the last updated by user
                    break;
                case eOperation.DELETE:
                    application.Deleted = true; // Assuming you have a property to mark as deleted
                    application.DeletedBy = User.Identity.Name; // Assuming you have a way to get the current user
                    application.DeletedDate = DateTime.UtcNow; // Assuming you have a property for the deletion date
                    break;
                case eOperation.UNDELETE:
                    application.Deleted = false; // Assuming you have a property to mark as deleted
                    application.LastUpdatedBy = User.Identity.Name; // Assuming you have a way to get the current user
                    application.LastUpdateDate = DateTime.UtcNow; // Assuming you have a property for the deletion date
                    break;
                default:
                    break;
            }
        }
    }
}