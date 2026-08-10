using School.BLL.Interfaces;
using School.DAL.Interfaces;

namespace School.BLL
{
    public class OwnershipService : IOwnershipService
    {
        private readonly IStudentData _studentData;
        private readonly IParentData _parentData;
        private readonly IStudentParentData _studentParentData;

        public OwnershipService(IStudentData studentData, IParentData parentData, IStudentParentData studentParentData)
        {
            _studentData = studentData;
            _parentData = parentData;
            _studentParentData = studentParentData;
        }

        public async Task<bool> IsOwnStudentAsync(int studentId, int currentPersonId)
        {
            var student = await _studentData.GetStudentByIdAsync(studentId);
            if (student == null)
                return false;

            if (student.PersonID == currentPersonId)
                return true;

            var parent = await _parentData.GetParentByPersonIdAsync(currentPersonId);
            if (parent == null)
                return false;

            var parents = await _studentParentData.GetParentsByStudentIdAsync(studentId);
            return parents.Any(p => p.ParentID == parent.ParentID);
        }

        public async Task<bool> IsOwnParentRecordAsync(int parentId, int currentPersonId)
        {
            var parent = await _parentData.GetParentByIdAsync(parentId);
            return parent != null && parent.PersonID == currentPersonId;
        }
    }
}